$(function () {
    'use strict';

    $(".check-box.bootstrap-switch")
        .not($(".check-box.bootstrap-switch.yes-no").bootstrapSwitch({
            onText: 'YES',
            offText: 'NO'
        }))
        .bootstrapSwitch();

    $(document.body).addClass('in');
})