
-- USE [ASPState]
-- GO

CREATE TABLE ASPStateTempSessions (
    SessionId           nvarchar(88)    NOT NULL PRIMARY KEY,
    Created             datetime        NOT NULL DEFAULT GETUTCDATE(),
    Expires             datetime        NOT NULL,
    LockDate            datetime        NOT NULL,
    LockDateLocal       datetime        NOT NULL,
    LockCookie          int             NOT NULL,
    Timeout             int             NOT NULL,
    Locked              bit             NOT NULL,
    SessionItemShort    VARBINARY(7000) NULL,
    SessionItemLong     varbinary(MAX)  NULL,
    Flags               int             NOT NULL DEFAULT 0,
) 

CREATE NONCLUSTERED INDEX Index_Expires ON ASPStateTempSessions(Expires)

CREATE TABLE ASPStateTempApplications (
    AppId               int             NOT NULL PRIMARY KEY,
    AppName             char(280)       NOT NULL,
) 

CREATE NONCLUSTERED INDEX Index_AppName ON ASPStateTempApplications(AppName)


GO   

CREATE PROCEDURE TempGetVersion
    @ver      char(10) OUTPUT
AS
    SELECT @ver = '2'
    RETURN 0
GO

/*****************************************************************************/

CREATE PROCEDURE GetHashCode
    @input varchar(280),
    @hash int OUTPUT
AS
    /* 
       This sproc is based on this C# hash function:

        int GetHashCode(string s)
        {
            int     hash = 5381;
            int     len = s.Length;

            for (int i = 0; i < len; i++) {
                int     c = Convert.ToInt32(s[i]);
                hash = ((hash << 5) + hash) ^ c;
            }

            return hash;
        }

        However, SQL 7 doesn't provide a 32-bit integer
        type that allows rollover of bits, we have to
        divide our 32bit integer into the upper and lower
        16 bits to do our calculation.
    */
       
    DECLARE @hi_16bit   int
    DECLARE @lo_16bit   int
    DECLARE @hi_t       int
    DECLARE @lo_t       int
    DECLARE @len        int
    DECLARE @i          int
    DECLARE @c          int
    DECLARE @carry      int

    SET @hi_16bit = 0
    SET @lo_16bit = 5381
    
    SET @len = DATALENGTH(@input)
    SET @i = 1
    
    WHILE (@i <= @len)
    BEGIN
        SET @c = ASCII(SUBSTRING(@input, @i, 1))

        /* Formula:                        
           hash = ((hash << 5) + hash) ^ c */

        /* hash << 5 */
        SET @hi_t = @hi_16bit * 32 /* high 16bits << 5 */
        SET @hi_t = @hi_t & 0xFFFF /* zero out overflow */
        
        SET @lo_t = @lo_16bit * 32 /* low 16bits << 5 */
        
        SET @carry = @lo_16bit & 0x1F0000 /* move low 16bits carryover to hi 16bits */
        SET @carry = @carry / 0x10000 /* >> 16 */
        SET @hi_t = @hi_t + @carry
        SET @hi_t = @hi_t & 0xFFFF /* zero out overflow */

        /* + hash */
        SET @lo_16bit = @lo_16bit + @lo_t
        SET @hi_16bit = @hi_16bit + @hi_t + (@lo_16bit / 0x10000)
        /* delay clearing the overflow */

        /* ^c */
        SET @lo_16bit = @lo_16bit ^ @c

        /* Now clear the overflow bits */	
        SET @hi_16bit = @hi_16bit & 0xFFFF
        SET @lo_16bit = @lo_16bit & 0xFFFF

        SET @i = @i + 1
    END

    /* Do a sign extension of the hi-16bit if needed */
    IF (@hi_16bit & 0x8000 <> 0)
        SET @hi_16bit = 0xFFFF0000 | @hi_16bit

    /* Merge hi and lo 16bit back together */
    SET @hi_16bit = @hi_16bit * 0x10000 /* << 16 */
    SET @hash = @hi_16bit | @lo_16bit

    RETURN 0
GO
/*****************************************************************************/

CREATE PROCEDURE TempGetAppID
    @appName    varchar(280),
    @appId      int OUTPUT
    AS
    BEGIN
		SET @appName = LOWER(@appName)
		SET @appId = NULL

		SELECT @appId = AppId
		FROM ASPStateTempApplications
		WHERE AppName = @appName

		IF @appId IS NULL 
		BEGIN
			BEGIN TRAN        

			SELECT @appId = AppId
			FROM ASPStateTempApplications WITH (TABLOCKX)
			WHERE AppName = @appName
	        
			IF @appId IS NULL
			BEGIN
				EXEC GetHashCode @appName, @appId OUTPUT
	            
				INSERT ASPStateTempApplications
				VALUES
				(@appId, @appName)
	            
				IF @@ERROR = 2627 
				BEGIN
					DECLARE @dupApp varchar(280)
	            
					SELECT @dupApp = RTRIM(AppName)
					FROM ASPStateTempApplications 
					WHERE AppId = @appId
	                
					RAISERROR('SQL session state fatal error: hash-code collision between applications ''%s'' and ''%s''. Please rename the 1st application to resolve the problem.', 
								18, 1, @appName, @dupApp)
				END
			END

			COMMIT
		END

		RETURN 0
    END

GO

/*****************************************************************************/

CREATE PROCEDURE TempGetStateItem
    @id         nvarchar(88),
    @itemShort  varbinary(7000) OUTPUT,
    @locked     bit OUTPUT,
    @lockDate   datetime OUTPUT,
    @lockCookie int OUTPUT
AS
BEGIN
    DECLARE @textptr AS varbinary(16)
    DECLARE @length AS int
    DECLARE @now AS datetime
    SET @now = GETUTCDATE()

    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, @now), 
        @locked = Locked,
        @lockDate = LockDateLocal,
        @lockCookie = LockCookie,
        @itemShort = CASE @locked
            WHEN 0 THEN SessionItemShort
            ELSE NULL
            END,
        @length = CASE @locked
            WHEN 0 THEN DATALENGTH(SessionItemLong)
            ELSE NULL
            END
    WHERE SessionId = @id
    
    IF @length IS NOT NULL 
    BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
    END

    RETURN 0
END

GO
/*****************************************************************************/

CREATE PROCEDURE TempGetStateItem2
	@id         nvarchar(88),
	@itemShort  varbinary(7000) OUTPUT,
	@locked     bit OUTPUT,
	@lockAge    int OUTPUT,
	@lockCookie int OUTPUT
AS
BEGIN
	DECLARE @textptr AS varbinary(16)
	DECLARE @length AS int
	DECLARE @now AS datetime
	SET @now = GETUTCDATE()

	UPDATE ASPStateTempSessions
	SET Expires = DATEADD(n, Timeout, @now), 
		@locked = Locked,
		@lockAge = DATEDIFF(second, LockDate, @now),
		@lockCookie = LockCookie,
		@itemShort = CASE @locked
			WHEN 0 THEN SessionItemShort
			ELSE NULL
			END,
		@length = CASE @locked
			WHEN 0 THEN DATALENGTH(SessionItemLong)
			ELSE NULL
			END
	WHERE SessionId = @id
	IF @length IS NOT NULL 
	BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
	END

	RETURN 0
END
GO
/*****************************************************************************/

CREATE PROCEDURE TempGetStateItem3
    @id         nvarchar(88),
    @itemShort  varbinary(7000) OUTPUT,
    @locked     bit OUTPUT,
    @lockAge    int OUTPUT,
    @lockCookie int OUTPUT,
    @actionFlags int OUTPUT
AS
BEGIN
    DECLARE @textptr AS varbinary(16)
    DECLARE @length AS int
    DECLARE @now AS datetime
    SET @now = GETUTCDATE()

    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, @now), 
        @locked = Locked,
        @lockAge = DATEDIFF(second, LockDate, @now),
        @lockCookie = LockCookie,
        @itemShort = CASE @locked
            WHEN 0 THEN SessionItemShort
            ELSE NULL
            END,
        @length = CASE @locked
            WHEN 0 THEN DATALENGTH(SessionItemLong)
            ELSE NULL
            END,

        /* If the Uninitialized flag (0x1) if it is set,
           remove it and return InitializeItem (0x1) in actionFlags */
        Flags = CASE
            WHEN (Flags & 1) <> 0 THEN (Flags & ~1)
            ELSE Flags
            END,
        @actionFlags = CASE
            WHEN (Flags & 1) <> 0 THEN 1
            ELSE 0
            END
    WHERE SessionId = @id
    IF @length IS NOT NULL 
    BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
    END

    RETURN 0
END       
GO
/*****************************************************************************/

CREATE PROCEDURE TempGetStateItemExclusive
    @id         nvarchar(88),
    @itemShort  varbinary(7000) OUTPUT,
    @locked     bit OUTPUT,
    @lockDate   datetime OUTPUT,
    @lockCookie int OUTPUT
AS
BEGIN
    DECLARE @textptr AS varbinary(16)
    DECLARE @length AS int
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime

    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()
    
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, @now), 
        LockDate = CASE Locked
            WHEN 0 THEN @now
            ELSE LockDate
            END,
        @lockDate = LockDateLocal = CASE Locked
            WHEN 0 THEN @nowLocal
            ELSE LockDateLocal
            END,
        @lockCookie = LockCookie = CASE Locked
            WHEN 0 THEN LockCookie + 1
            ELSE LockCookie
            END,
        @itemShort = CASE Locked
            WHEN 0 THEN SessionItemShort
            ELSE NULL
            END,
        @length = CASE Locked
            WHEN 0 THEN DATALENGTH(SessionItemLong)
            ELSE NULL
            END,
        @locked = Locked,
        Locked = 1
    WHERE SessionId = @id
    IF @length IS NOT NULL 
    BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
    END

    RETURN 0
END


GO
/*****************************************************************************/

CREATE PROCEDURE TempGetStateItemExclusive2
    @id         nvarchar(88),
    @itemShort  varbinary(7000) OUTPUT,
    @locked     bit OUTPUT,
    @lockAge    int OUTPUT,
    @lockCookie int OUTPUT
AS
BEGIN
    DECLARE @textptr AS varbinary(16)
    DECLARE @length AS int
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime

    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()
    
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, @now), 
        LockDate = CASE Locked
            WHEN 0 THEN @now
            ELSE LockDate
            END,
        LockDateLocal = CASE Locked
            WHEN 0 THEN @nowLocal
            ELSE LockDateLocal
            END,
        @lockAge = CASE Locked
            WHEN 0 THEN 0
            ELSE DATEDIFF(second, LockDate, @now)
            END,
        @lockCookie = LockCookie = CASE Locked
            WHEN 0 THEN LockCookie + 1
            ELSE LockCookie
            END,
        @itemShort = CASE Locked
            WHEN 0 THEN SessionItemShort
            ELSE NULL
            END,
        @length = CASE Locked
            WHEN 0 THEN DATALENGTH(SessionItemLong)
            ELSE NULL
            END,
        @locked = Locked,
        Locked = 1
    WHERE SessionId = @id
    IF @length IS NOT NULL 
    BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
    END

    RETURN 0
END
GO
/*****************************************************************************/

CREATE PROCEDURE TempGetStateItemExclusive3
    @id         nvarchar(88),
    @itemShort  varbinary(7000) OUTPUT,
    @locked     bit OUTPUT,
    @lockAge    int OUTPUT,
    @lockCookie int OUTPUT,
    @actionFlags int OUTPUT
AS
BEGIN
    DECLARE @textptr AS varbinary(16)
    DECLARE @length AS int
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime

    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()
    
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, @now), 
        LockDate = CASE Locked
            WHEN 0 THEN @now
            ELSE LockDate
            END,
        LockDateLocal = CASE Locked
            WHEN 0 THEN @nowLocal
            ELSE LockDateLocal
            END,
        @lockAge = CASE Locked
            WHEN 0 THEN 0
            ELSE DATEDIFF(second, LockDate, @now)
            END,
        @lockCookie = LockCookie = CASE Locked
            WHEN 0 THEN LockCookie + 1
            ELSE LockCookie
            END,
        @itemShort = CASE Locked
            WHEN 0 THEN SessionItemShort
            ELSE NULL
            END,
        @length = CASE Locked
            WHEN 0 THEN DATALENGTH(SessionItemLong)
            ELSE NULL
            END,
        @locked = Locked,
        Locked = 1,

        /* If the Uninitialized flag (0x1) if it is set,
           remove it and return InitializeItem (0x1) in actionFlags */
        Flags = CASE
            WHEN (Flags & 1) <> 0 THEN (Flags & ~1)
            ELSE Flags
            END,
        @actionFlags = CASE
            WHEN (Flags & 1) <> 0 THEN 1
            ELSE 0
            END
    WHERE SessionId = @id
    IF @length IS NOT NULL 
    BEGIN
        Select SessionItemLong from ASPStateTempSessions Where SessionId = @id
    END

    RETURN 0
END

Go

/*****************************************************************************/
CREATE PROCEDURE TempReleaseStateItemExclusive
    @id         nvarchar(88),
    @lockCookie int
AS
	BEGIN
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, GETUTCDATE()), 
        Locked = 0
    WHERE SessionId = @id AND LockCookie = @lockCookie

    RETURN 0
    END
GO
/*****************************************************************************/

CREATE PROCEDURE TempInsertUninitializedItem
    @id         nvarchar(88),
    @itemShort  varbinary(7000),
    @timeout    int
AS    
BEGIN
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime
    
    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()

    INSERT ASPStateTempSessions 
        (SessionId, 
         SessionItemShort, 
         Timeout, 
         Expires, 
         Locked, 
         LockDate,
         LockDateLocal,
         LockCookie,
         Flags) 
    VALUES 
        (@id, 
         @itemShort, 
         @timeout, 
         DATEADD(n, @timeout, @now), 
         0, 
         @now,
         @nowLocal,
         1,
         1)

    RETURN 0
END    
GO
/*****************************************************************************/

CREATE PROCEDURE TempInsertStateItemShort
    @id         nvarchar(88),
    @itemShort  varbinary(7000),
    @timeout    int
AS    
BEGIN
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime
    
    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()

    INSERT ASPStateTempSessions 
        (SessionId, 
         SessionItemShort, 
         Timeout, 
         Expires, 
         Locked, 
         LockDate,
         LockDateLocal,
         LockCookie) 
    VALUES 
        (@id, 
         @itemShort, 
         @timeout, 
         DATEADD(n, @timeout, @now), 
         0, 
         @now,
         @nowLocal,
         1)

    RETURN 0
END
GO
/*****************************************************************************/

CREATE PROCEDURE TempInsertStateItemLong
    @id         nvarchar(88),
    @itemLong   varbinary(max),
    @timeout    int
AS   
BEGIN 
    DECLARE @now AS datetime
    DECLARE @nowLocal AS datetime
    
    SET @now = GETUTCDATE()
    SET @nowLocal = GETDATE()

    INSERT ASPStateTempSessions 
        (SessionId, 
         SessionItemLong, 
         Timeout, 
         Expires, 
         Locked, 
         LockDate,
         LockDateLocal,
         LockCookie) 
    VALUES 
        (@id, 
         @itemLong, 
         @timeout, 
         DATEADD(n, @timeout, @now), 
         0, 
         @now,
         @nowLocal,
         1)

    RETURN 0
END    
GO
/*****************************************************************************/

CREATE PROCEDURE TempUpdateStateItemShort
    @id         nvarchar(88),
    @itemShort  varbinary(7000),
    @timeout    int,
    @lockCookie int
AS 
BEGIN   
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, @timeout, GETUTCDATE()), 
        SessionItemShort = @itemShort, 
        Timeout = @timeout,
        Locked = 0
    WHERE SessionId = @id AND LockCookie = @lockCookie

    RETURN 0
END

GO
/*****************************************************************************/

CREATE PROCEDURE TempUpdateStateItemShortNullLong
    @id         nvarchar(88),
    @itemShort  varbinary(7000),
    @timeout    int,
    @lockCookie int
AS  
BEGIN  
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, @timeout, GETUTCDATE()), 
        SessionItemShort = @itemShort, 
        SessionItemLong = NULL, 
        Timeout = @timeout,
        Locked = 0
    WHERE SessionId = @id AND LockCookie = @lockCookie

    RETURN 0
END    
GO
/*****************************************************************************/

CREATE PROCEDURE TempUpdateStateItemLong
    @id         nvarchar(88),
    @itemLong   varbinary(max),
    @timeout    int,
    @lockCookie int
AS 
BEGIN   
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, @timeout, GETUTCDATE()), 
        SessionItemLong = @itemLong,
        Timeout = @timeout,
        Locked = 0
    WHERE SessionId = @id AND LockCookie = @lockCookie

    RETURN 0
END

GO
/*****************************************************************************/

CREATE PROCEDURE TempUpdateStateItemLongNullShort
    @id         nvarchar(88),
    @itemLong   varbinary(max),
    @timeout    int,
    @lockCookie int
AS
BEGIN    
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, @timeout, GETUTCDATE()), 
        SessionItemLong = @itemLong, 
        SessionItemShort = NULL,
        Timeout = @timeout,
        Locked = 0
    WHERE SessionId = @id AND LockCookie = @lockCookie

    RETURN 0
END            
GO

/*****************************************************************************/

CREATE PROCEDURE TempRemoveStateItem
    @id     nvarchar(88),
    @lockCookie int
AS
BEGIN
    DELETE ASPStateTempSessions
    WHERE SessionId = @id AND LockCookie = @lockCookie
    RETURN 0
END
GO
/*****************************************************************************/


CREATE PROCEDURE TempResetTimeout
    @id     nvarchar(88)
AS
BEGIN
    UPDATE ASPStateTempSessions
    SET Expires = DATEADD(n, Timeout, GETUTCDATE())
    WHERE SessionId = @id
    RETURN 0 
END
GO
/*****************************************************************************/

CREATE PROCEDURE DeleteExpiredSessions
AS
BEGIN
    DECLARE @now datetime
    DECLARE @nbCount int;
    
    
	Begin Transaction
	
	SET @now = GETUTCDATE()

	Select @nbCount = COUNT(*) from ASPStateTempSessions where Expires < @now;
	
    DELETE ASPStateTempSessions WHERE Expires < @now

	Select  @nbCount as DeletedSessions ;

	Commit Transaction

    RETURN 0 
END
GO
/*****************************************************************************/
