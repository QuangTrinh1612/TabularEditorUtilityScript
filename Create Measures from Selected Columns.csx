// Creates a SUM measure for every currently selected column and hide the column.
foreach(var c in Selected.Columns)
{
    var newMeasure = c.Table.AddMeasure(
        "Sum of " + c.Name,                    // Name
        "SUM(" + c.DaxObjectFullName + ")",    // DAX expression
        c.DisplayFolder                        // Display Folder
    );
    
    // Set the format string on the new measure:
    newMeasure.FormatString = "#,0";

    // Provide some documentation:
    var desc = newMeasure.Expression;

    newMeasure.Description = "Formula: "+desc;

    // Hide the base column:
    c.IsHidden = true;
}