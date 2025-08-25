$(document).ready(function () {

    // MediaRecorder instance
    var mediaRecorder = null;

    // Send email.
    $('#sendEmail').click(function sendEmail() {
        // Implement email sending functionality here
        alert("Email sent!");
    });

    // Record from microphone
    $("#recordVoice").click(function () {

        // If already recording, stop the recording
        if (mediaRecorder != null) {
            mediaRecorder.stop();
            mediaRecorder = null;
            $("#recordVoice").text(properties.recordVoice);
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
                    // You can now upload or play the blob
                    // Example: createObjectURL(blob) and play in <audio>
                    mediaRecorder = null;
                    $("#recordVoice").text(properties.recordVoice);
                };

                // Start recording
                mediaRecorder.start();
                $("#recordVoice").text(properties.stopRecording);

                // Stop after @(MaxRecTime * 1000) seconds.
                setTimeout(() => {

                    if (mediaRecorder != null) {
                        mediaRecorder.stop();
                        mediaRecorder = null
                    }
                    $("#recordVoice").text(properties.recordVoice);

                }, properties.maxRecTime * 1000);
            })
            .catch(function (err) {
                console.error(properties.microphoneError + " " + err);
            });
    });
})
