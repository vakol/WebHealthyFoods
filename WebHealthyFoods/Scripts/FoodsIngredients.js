$(document).ready(function () {

    // Populate the dropdown with food items
    var dropdown = $('#FoodList');
    $.each(properties.foods, function () {
        dropdown.append($('<option />').val(this.identifikace_suroviny).text(this.popis_suroviny));
    });
    // Handle change event for the AddToList button
    $('#AddToList').click (function () {
        // Get the selected food item ID and add it to the list
        var selectedFoodId = dropdown.val();
        // Check if the item is already in the list
        if ($('#SelectedFoodList li[data-id="' + selectedFoodId + '"]').length > 0) {
            alert(properties.alreadyInList);
            return;
        }
        // Add the selected item to the list
        $('#SelectedFoodList').append($('<li class="list-group-item" />').text($('#FoodList option:selected').text()).attr('data-id', selectedFoodId));

        //  Clear ingredient details
        $('#IngredientsList').empty();
        // Find all ingredient IDs for all selected food IDs.
        var foodsIngredientIds = [];
        $('#SelectedFoodList li').each(function () {
            var foodId = $(this).attr('data-id')
            // Find the food ingredients based on food ID.
            properties.foodIngredients.forEach(function (foodIngredient) {
                if (foodIngredient.identifikace_suroviny == foodId) {
                    foodsIngredientIds.push(foodIngredient.identifikace_slozky);
                }
            });
        });
        // Find the ingredient details based on ingredient ID.
        foodsIngredientIds.map(function (ingredientId) {
            properties.ingredients.forEach(function (ingredient) {
                if (ingredient.identifikace_slozky == ingredientId) {
                    // Add the ingredient to the list if not already present
                    if ($('#IngredientsList li[data-id="' + ingredientId + '"]').length === 0) {
                        var ingredientEffect = ingredient.pusobeni_slozky != "-" ? " (" + ingredient.pusobeni_slozky + ")": "";
                        $('#IngredientsList').append($('<li class="list-group-item" />').text(ingredient.nazev_slozky + ingredientEffect).attr('data-id', ingredientId));
                    }
                }
            });
        });
    });
});