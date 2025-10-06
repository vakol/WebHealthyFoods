
// Helper function that gets cookie or null value if it doesn't exist.
function getCookie(name) {
    var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) return match[2];
    return null;
}

// Helper function that sets cookie.
function setCookieInternal(name, value, days, checkUserConfirm) {
    // Remove cookies if cookies are disabled.
    if (checkUserConfirm && (getCookie(ENABLE_COOKIE_NAME) !== "true")) {
        removeCookies()
        return;
    }
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// Helper functions that sets cookie.
function setCookie(name, value, days) {
    // Delegate to the main function with user confirmation check.
    setCookieInternal(name, value, days, true);
}
function setCookieNoCheck(name, value, days) {
    // Delegate to the main function without check.
    setCookieInternal(name, value, days, false);
}

// Remove all cookies for the current site
function removeCookies() {
    document.cookie.split(";").forEach(function (cookie) {
        var name = cookie.split("=")[0].trim();
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/';
    });
}

// Helper constants to store banner ID and enabling cookie.
const BANNER_ID = "cookies-banner";
const ENABLE_COOKIE_NAME = "cookies_enabled";

// Helper function that enables cookies on the site.
function enableCookies(enable) {
    if (enable) {
        setCookieNoCheck(ENABLE_COOKIE_NAME, "true", 30);  // Saved for 30 days.
    }
    $('#' + BANNER_ID).addClass('d-none');
}

// Helper function that disables cookies banner on the site on document loading.
$(document).ready(function ()
{
    if (getCookie(ENABLE_COOKIE_NAME) === "true") {
        return;
    }

    // If cookies are disabled, remove all cookies.
    removeCookies();
    // Display cookies banner if cookie is not set.
    var bannerDisplayed = $("#" + BANNER_ID).css("display");
    if (bannerDisplayed === "none") {
        $('#' + BANNER_ID).removeClass('d-none');
    }
});

// Helper function that sends e-mail contents to server app.
function sendEmail(serverUrl, emailAddress, subject, message) {
    // Check e-mail address.
    if (emailAddress == "") {
        alert(utilityProp.enterEmail);
        return;
    }
    // Post e-mail contents to the server.
    $.ajax({
        url: serverUrl,
        type: 'POST',
        data: {
            emailAddress: emailAddress,
            subject: subject,
            message: message
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
            }
            else {
                alert(utilityProp.emailError);
            }
        },
        error: function () {
            alert(utilityProp.emailError);
        }
    });
}