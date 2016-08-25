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
        var target = $(this).data('target') || '_self';
        if (event.target.tagName != 'A' && href) {
            window.open(href, target, true);
        }
    });

    $('.dropdown input[type="hidden"]').each(function (i, input) {
        var dropdown = $(this).closest('.dropdown');
        $('a[data-value]', dropdown).click(function (event) {
            event.preventDefault();
            var value = $(this).data('value');
            var label = $(this).text();
            var toggle = $('[data-toggle="dropdown"]', dropdown);
            var caret = $('.caret', toggle);
            toggle.text(label + ' ');
            caret.length && toggle.append(caret);
            input.value = value;
        });
    });

    $(document.body).addClass('in');
})