$(document).ready(function () {
    
    // MediaRecorder instance
    var mediaRecorder = null;
    // Variable to hold the modal dialog instance.
    var sendEmailModal = null;
    // Line separator for email contents.
    const LINE = "-----------------------------------------\n";

    // Send email.
    $('#send-email').click(function sendEmail() {
        // Get food type.
        var foodType = $('#food-type').val();
        // Get food name.
        var foodName = $('#food-name').val();
        // Get food preparation time.
        var foodTime = $('#food-time').val();
        // Get food energy value.
        var foodEnergy = $('#food-energy').val();
        // Get food preparation.
        var foodPrepar = $('#current-prep').val();
        // Get food notes.
        var foodNotes = $('#food-notes').val();
        // Compile email contents.
        var emailContents =
                'doba ' + foodTime + ", energie " + foodEnergy + "\n" +
                LINE +
                foodPrepar;
        if (foodNotes != "") {
            emailContents +=
                "\n" +
                LINE +
                foodNotes;
        }
        // Show the modal dialog.
        $("#email-food").val(emailContents);
        var userEmail = getCookie('user_email');
        if (userEmail != null) {
            $("#email-address").val(userEmail);
        }
        $("#email-subject").val("HealthyFoods - " + foodType + ": \"" + foodName + "\"");
        sendEmailModal = new bootstrap.Modal(document.getElementById('send-email-modal'));
        sendEmailModal.show();
    });

    // Send e-mail contents.
    $("#send-email-contents").click(function () {
        // Get e-mail contents and e-mail address.
        var userEmail = $("#email-address").val();
        if (userEmail != "") {
            setCookie('user_email', userEmail, 365);
        }
        var emailSubject = $("#email-subject").val();
        var foodType = $('#food-type').val();
        var foodName = $('#food-name').val();
        var emailContents = LINE + '\"' + foodName + '\" ' + foodType + '\n' + LINE +$("#email-food").val();
        // Post e-mail contents to the server.
        sendEmail('/Home/SendEmail', userEmail, emailSubject, emailContents);
        if (sendEmailModal != null) {
            sendEmailModal.hide();
        }
    });

    // Record from microphone
    $('#record-voice').click(function () {

        // If already recording, stop the recording
        if (mediaRecorder != null) {
            mediaRecorder.stop();
            mediaRecorder = null;
            $('#record-voice').text(properties.recordVoice);
            return;
        }

        // Inform the user about the recording duration
        alert(properties.informUser);
        navigator.mediaDevices.getUserMedia({ audio: true })
            .then(function (stream) {

                // Set up MediaRecorder
                mediaRecorder = new MediaRecorder(stream);
                let chunks = [];

                // Load data when available
                mediaRecorder.ondataavailable = function (e) {
                    chunks.push(e.data);
                };

                // When recording stops, create a blob from the chunks
                mediaRecorder.onstop = function (e) {

                    const blob = new Blob(chunks, { 'type': 'audio/ogg; codecs=opus' });
                    // Upload the blob with AJAX
                    var formData = new FormData();
                    formData.append('voice_action', 'ogg2text'); // Add your extra parameter here
                    formData.append('file', blob, 'voice' + properties.uniqueRequestId + '.ogg');

                    var urlAction = properties.voice2TextUrl;

                    $.ajax({
                        url: urlAction,
                        type: 'POST',
                        data: formData,
                        processData: false, // Don't process the data
                        contentType: false, // Let the browser set Content-Type
                        success: function (voice2textResponse) {
                            if (voice2textResponse !== undefined) {
                                $('#food-notes').val(voice2textResponse.trim());
                            }
                            if (mediaRecorder != null) {
                                mediaRecorder.stop();
                                setTimeout(() => {
                                    mediaRecorder = null;
                                }, 200);
                            }
                        },
                        error: function (xhr, status, error) {
                            if (status !== undefined && error !== undefined) {
                                $('#food-notes').val('ERROR: ' + error.trim() + ' (' + status + ')');
                            }
                            if (mediaRecorder != null) {
                                mediaRecorder.stop();
                                setTimeout(() => {
                                    mediaRecorder = null;
                                }, 200);
                            }
                        }
                    });
                    
                    mediaRecorder = null;
                    $('#record-voice').text(properties.recordVoice);
                };

                // Start recording
                mediaRecorder.start();
                $('#record-voice').text(properties.stopRecording);

                // Stop after @(MaxRecTime * 1000) seconds.
                setTimeout(() => {

                    if (mediaRecorder != null) {
                        mediaRecorder.stop();
                        mediaRecorder = null;
                    }
                    $('#record-voice').text(properties.recordVoice);

                }, properties.maxRecTime * 1000);
            })
            .catch(function (err) {
                console.error(properties.microphoneError + ' ' + err);
            });
    });
})
