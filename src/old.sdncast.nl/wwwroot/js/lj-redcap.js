$(document).ready(function () {
    'use strict';

    // E-mail validation via regular expression
    function isValidEmailAddress(emailAddress) {
        var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
        return pattern.test(emailAddress);
    }

    // Subscription notifications 
    $(function () {
        $('.lj-message').hide();
        $("#newsletter").submit(function (event) {
            var input = $('.lj-message');
            if (!input.is(':empty')) {
                $('.lj-message').stop(true).fadeOut(250);
            }
            event.preventDefault();
            event.stopImmediatePropagation();

            var email = $("input#email").val();

            if (email === "") {

                $(".lj-message").stop(true).html('<i class="fa fa-exclamation-circle"></i> You must enter a valid e-mail address.').fadeIn(250);
                $("input#email").focus();
            }
            else if (!isValidEmailAddress(email)) {
                $(".lj-message").stop(true).html('<i class="fa fa-exclamation-circle"></i> E-mail address is not valid.').fadeIn(250);
                $("input#email").focus();
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "./php/send.php",
                    data: { subscription: email },
                    success: function () {
                        $("#newsletter").fadeOut(250, function () {
                            $(".lj-message").html('<i class="fa fa-check-circle"></i> We will be in touch soon!').fadeIn(250);
                        });
                    }
                });
            }
        });
    });

    // Scroll to newsletter 
    // Function that scrolls to newsletter form placed on the bottom
    // $(".lj-header-button a").click(function(e) { 
    //   e.preventDefault();
    //   $('html,body').animate({
    //     scrollTop: $("#newsletter-form").offset().top},
    //     'slow');           
    // });



    // backstretch
    $("header").backstretch("/images/home-bg.jpg");
});
