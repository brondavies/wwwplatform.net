$(function () {
    'use strict';

    $(".check-box.bootstrap-switch")
        .not($(".check-box.bootstrap-switch.yes-no").bootstrapSwitch({
            onText: 'YES',
            offText: 'NO'
        }))
        .bootstrapSwitch();

    var failure = '_failure';
    var success = '_success';
    var failureMessage = $.cookie(failure);
    if (failureMessage) {
        $.removeCookie(failure);
        $.removeCookie(failure, { path: '/' });
        $('#messages, .messages').first().append(
            $('<p></p>').html(failureMessage).addClass('bg-danger text-danger')
        );
    }
    var successMessage = $.cookie(success);
    if (successMessage) {
        $.removeCookie(success);
        $.removeCookie(success, { path: '/' });
        $('#messages, .messages').first().append(
            $('<p></p>').html(successMessage).addClass('bg-success text-success')
        );
    }

    $('#messages, .messages').click(function () {
        $('p', this).animate({ opacity: 0 },
            function () {
                $(this).remove();
            });
    });

    $('.auto-datatable').DataTable();

    $(document).on('click', '[data-href]', function (event) {
        var href = $(this).data('href');
        if (event.target.tagName != 'A' && href) {
            window.open(href, '_blank', true);
        }
    });

    $(document.body).addClass('in');
})