# Features
The primary reason to use wwwplatform&#42;net is to get to your custom development as quickly as possible while implementing a full-featured site with administrative functionality out of the box.
- [Features](#features)
    - [Account Management](#account-management)
    - [User Roles](#user-roles)
    - [Pages](#pages)
    - [Shared Folders](#shared-folders)
    - [File upload](#file-upload)
    - [Mailing lists](#mailing-lists)

## Account Management
This feature implements simple user authentication for all user types.
- Sign in / sign out
- User registration
- Email confirmation
- Password change
- Password reset

## User Roles
Roles allow administrators to control access to specific areas and functionality of the site. The built-in roles and their purpose are listed here. You should not remove the built-in roles but you can add your own roles. Just go to the Admin menu and click on 'Manage Roles'
- Administrators - Admins have full access to the entire site.
- Users - Anyone with a login is part of the Users role.
- Editors - These users can manage the site pages.
- List Managers - These users can manage the mailing lists.
- Public - This is analogous to an anonymous user.

## Pages
Although not a full-fledged blogging system, site pages allow users in the Editors role to create and manage additional site content including the home page.
- Create/Edit/Delete pages
- Role-restricted pages
- Automatic listing in the site menu
- Parent pages
- [Custom layouts](CUSTOMIZING.md#custom-layouts)
- WYSIWYG editor
- External linking / redirecting

## Shared Folders
For simple file sharing and access delegation, shared folders allow you to upload, preview, link to and download files. You can turn this feature on/off or simply leave it accessible to anyone with the link.
- Create/Edit/Delete shared folders
- Role-restricted folders and files
- Folders within folders
- File sharing in multiple folders
- Drag & drop upload

## File upload
Files can be uploaded by users to the site where access is granted.  Users have their own library of files that can be re-used in multiple places. Instead of uploading a file multiple times, the file can be selected from the user's library anywhere you see a file upload dialog.
- Drag & drop upload
- File select / upload dialog
- Role-restricted file access
- File library management per user (under account management)

## Mailing lists
Like a list-serve, mailing lists allow you to send bulk email messages to individuals who subscribe but only one-way from the site interface. Capabilities include:
- Subscribe
- Email confirmation
- Unsubscribe
- Send bulk email
- List management

Users in the List Managers role can send emails to the lists and manage subscribers. This functionality is available by opening the Admin menu and clicking on 'Mailing Lists'
