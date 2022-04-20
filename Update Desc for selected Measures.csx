foreach(var m in Selected.Measures)
{
    var name = m.Name;
    var desc = m.Expression;
    m.Description = "Formula: "+desc;
}