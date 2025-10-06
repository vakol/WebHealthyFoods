$(document).ready(function () {

    // Update food list
    function updateFoodList() {

        //  Clear food details
        $('#food-list').empty();
        // Find all food IDs for all selected ingredient IDs.
        var foodIds = [];
        $('#selected-ingredient-list li').each(function () {
            var ingredientId = $(this).attr('data-id');
            // Find the foods based on ingredient ID.
            properties.foodIngredients.forEach(function (foodIngredient) {
                if (foodIngredient.identifikace_slozky == ingredientId) {
                    foodIds.push(foodIngredient.identifikace_suroviny);
                }
            });
        });
        // Find the food details based on food IDs.
        var areFoodsFound = false;
        foodIds.map(function (foodId) {
            properties.foods.forEach(function (food) {
                if (food.identifikace_suroviny == foodId) {
                    // Add the food to the list if not already present
                    if ($('#food-list li[data-id="' + foodId + '"]').length === 0) {
                        $('#food-list').append($('<li class="list-group-item" />').text(food.popis_suroviny).attr('data-id', foodId));
                        areFoodsFound = true;
                    }
                }
            });
        });
        // Display caption and list.
        if (areFoodsFound) {
            $('#foods-caption').show();
            $('#food-list').show();
            $('#send-email-ingredients').show();
        }
    }

    // Populate the dropdown with in ingredients
    var dropdown = $('#ingredient-list');
    $.each(properties.ingredients, function () {
        dropdown.append($('<option />').val(this.identifikace_slozky).text(this.nazev_slozky));
    });

    // Handle change event for the AddToList button
    $('#add-to-list').click (function () {
        // Get the selected ingredient ID and add it to the list
        var selectedIngredientId = dropdown.val();
        // Check if the item is already in the list
        if ($('#selected-ingredient-list li[data-id="' + selectedIngredientId + '"]').length > 0) {
            alert(properties.alreadyInList);
            return;
        }
        // Add the selected ingredient to the list
        // Create the <li> element
        var $li = $('<li class="list-group-item" />')
            .text($('#ingredient-list option:selected').text())
            .attr('data-id', selectedIngredientId);
        // Create the <img> element to enable removal of the ingredient
        var $img = $('<img />')
            .attr('src', '/Content/img/remove_item.png')
            .attr('alt', 'Remove')
            .addClass('remove-ingredient')
            .css({ 'margin-left': '10px', 'cursor': 'pointer' });
        // Attach click event to the <img> to remove the ingredient from the list
        $img.click(function () {
            $(this).closest('li').remove();
            // Update food list
            updateFoodList();
        });
        // Append the <img> to the <li>
        $li.append($img);
        // Append the <li> to the list
        $('#selected-ingredient-list').append($li);

        // Update food list.
        updateFoodList();
    });

    // Variable to hold the modal dialog instance.
    var sendEmailModal = null;

    // Handle click event for the Send E-mail button.
    $('#send-email-ingredients').click(function () {

        var ingredientList = '';
        $('#selected-ingredient-list li').each(function () {
            var ingredientName = $(this).text();
            if (ingredientList !== '') {
                ingredientList += '\n';
            }
            ingredientList += ingredientName;
        });
        var foodList = '';
        $('#food-list li').each(function () {
            var foodName = $(this).text();
            if (foodList !== '') {
                foodList += '\n';
            }
            foodList += foodName;
        });
        // Compile email contents.
        const LINE = "-------------------------------------\n";
        var emailContents =
            LINE +
            'Složky:' + '\n' +
            LINE +
            ingredientList + '\n' +
            LINE +
            'Potraviny:' + '\n' +
            LINE +
            foodList;
        // Show the modal dialog.
        $("#email-food").val(emailContents);
        var userEmail = getCookie('user_email');
        if (userEmail != null) {
            $("#email-address").val(userEmail);
        }
        $("#email-subject").val("HealthyFoods - složky v potravinách");
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
        var emailContents = $("#email-food").val();
        // Post e-mail contents to the server.
        sendEmail('/Home/SendEmail', userEmail, emailSubject, emailContents);
        if (sendEmailModal != null) {
            sendEmailModal.hide();
        }
    });
});