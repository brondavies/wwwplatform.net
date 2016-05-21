$(function () {
    'use strict';

    $(".check-box.bootstrap-switch")
        .not($(".check-box.bootstrap-switch.yes-no").bootstrapSwitch({
            onText: 'YES',
            offText: 'NO'
        }))
        .bootstrapSwitch();

    var failureMessage = $.cookie('_failure');
    if (failureMessage) {
        $.removeCookie('_failure');
        $('#messages, .messages').first().append(
            $('<p></p>').html(failureMessage).addClass('bg-danger text-danger')
        );
    }
    var successMessage = $.cookie('_success');
    if (successMessage) {
        $.removeCookie('_success');
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

    $(document.body).addClass('in');
})