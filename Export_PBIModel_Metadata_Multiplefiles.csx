// Update this location to the destination folder on your computer
string folderName = @"C:\Users\ext.trinh.quang\Desktop\Central Dataset - Metadata";


/****************************************************************************************/
/***********************************Columns and Measures ********************************/

// Construct a list of all visible columns and measures:
var objects = Model.AllMeasures.Where(m => !m.Table.IsHidden || m.Table.IsHidden).Cast<ITabularNamedObject>()
      .Concat(Model.AllColumns.Where(c => !c.Table.IsHidden || c.Table.IsHidden));

// Get their properties in TSV format (tabulator-separated):
var tsv = ExportProperties(objects,"ObjectName,ObjectType,TableName,SourceColumn,DataType,Expression,HiddenFlag,FormatString,Description");

// (Optional) Output to screen (can then be copy-pasted into Excel):
// tsv.Output();

// ...or save the TSV to a file:
SaveFile(folderName + @"\Measures&Columns.tsv", tsv);



/**********************************************************************/
/***********************************RLS********************************/

var sb = new System.Text.StringBuilder();

// Headers
sb.Append("RoleName" + '\t' + "TableName" + '\t' + "FilterExpression");
sb.Append(Environment.NewLine);

foreach (var t in Model.Tables)
{
    foreach(var r in Model.Roles)
    {
        string rls = Model.Tables[t.Name].RowLevelSecurity[r.Name];
        if (!String.IsNullOrEmpty(rls))
        {
            sb.Append(r.Name + '\t' + t.Name + '\t' + rls);
            sb.Append(Environment.NewLine);
        }
    }
}

System.IO.File.WriteAllText(folderName + @"\RLS.tsv", sb.ToString());


/**********************************************************************/
/****************************RELATIONSHIPS*****************************/

sb = new System.Text.StringBuilder();

// Header
sb.Append("FromTable" + '\t' + "FromColumn" + '\t' + "ToTable" + '\t' + "ToColumn" + '\t' +
          "Active" + '\t' + "CrossFilteringBehavior" + '\t' + "RelationshipType" + '\t' + "SecurityFilteringBehavior" + '\t' + "RelyOnReferentialIntegrity");
sb.Append(Environment.NewLine);

foreach (var r in Model.Relationships)
{
    string act = string.Empty;
    string relType = string.Empty;
    string cfb = string.Empty;
    string sfb = string.Empty;
    string rori = string.Empty;
    
    if (r.IsActive)
    {
        act = "Yes";
    }
    else
    {
        act = "No";
    }
    
    if (r.FromCardinality == RelationshipEndCardinality.Many && r.ToCardinality ==  RelationshipEndCardinality.One)
    {
        relType = "Many-to-One";
    }
    else if (r.FromCardinality == RelationshipEndCardinality.Many && r.ToCardinality ==  RelationshipEndCardinality.Many)
    {
        relType = "Many-to-Many";
    }
    
    if (r.CrossFilteringBehavior == CrossFilteringBehavior.OneDirection)
    {
        cfb = "Single";
    }
    else if (r.CrossFilteringBehavior == CrossFilteringBehavior.BothDirections)
    {
        cfb = "Bi";
    }
    if (r.SecurityFilteringBehavior == SecurityFilteringBehavior.OneDirection)
    {
        sfb = "Single";
    }
    else if (r.SecurityFilteringBehavior == SecurityFilteringBehavior.BothDirections)
    {
        sfb = "Bi";
    }
    if (r.RelyOnReferentialIntegrity)
    {
        rori = "Yes";
    }
    else
    {
        rori = "No";
    }
    
    sb.Append(r.FromTable.Name + '\t' + r.FromColumn.Name + '\t' + r.ToTable.Name + '\t' + r.ToColumn.Name + '\t' +
    act + '\t' + cfb + '\t' + relType + '\t' + sfb + '\t' + rori);
    sb.Append(Environment.NewLine);
}

System.IO.File.WriteAllText(folderName + @"\Relationships.tsv", sb.ToString());

/**********************************************************************/
/******************************HIERARCHIES*****************************/

sb = new System.Text.StringBuilder();

// Headers
sb.Append("HierarchyName" + '\t' + "TableName" + '\t' + "ColumnName");
sb.Append(Environment.NewLine);

// Hierarchies
foreach (var h in Model.AllHierarchies.ToList())
{
    foreach (var lev in h.Levels.ToList())
    {
        sb.Append(h.Name + '\t' + h.Table.Name + '\t' + lev.Name);
        sb.Append(Environment.NewLine);
    }
}

System.IO.File.WriteAllText(folderName + @"\Hierarchies.tsv", sb.ToString());

/**********************************************************************/
/******************************PERSPECTIVES****************************/

sb = new System.Text.StringBuilder();

// Headers
sb.Append("TableName" + '\t' + "ObjectName" + '\t' + "ObjectType");

// Loop header for each perspective
foreach (var p in Model.Perspectives.ToList())
{
    sb.Append('\t' + p.Name);
}

sb.Append(Environment.NewLine);

// Measures
foreach (var o in Model.AllMeasures.ToList())
{
    sb.Append(o.Parent.Name + '\t' + o.Name + '\t' + "Measure");
    
    foreach (var p in Model.Perspectives.ToList())
    {
        string per = string.Empty;
        if (o.InPerspective[p])
        {
            per = "Yes";
        }
        else
        {
            per = "No";
        }
        sb.Append('\t' + per);
    }
    sb.Append(Environment.NewLine);
}

// Columns
foreach (var o in Model.AllColumns.ToList())
{
    sb.Append(o.Table.Name + '\t' + o.Name + '\t' + "Column");
    
    foreach (var p in Model.Perspectives.ToList())
    {
        string per = string.Empty;
        if (o.InPerspective[p])
        {
            per = "Yes";
        }
        else
        {
            per = "No";
        }
        sb.Append('\t' + per);
    }
    sb.Append(Environment.NewLine);
}

// Hierarchies
foreach (var o in Model.AllHierarchies.ToList())
{
    sb.Append(o.Parent.Name + '\t' + o.Name + '\t' + "Hierarchy");
    
    foreach (var p in Model.Perspectives.ToList())
    {
        string per = string.Empty;
        if (o.InPerspective[p])
        {
            per = "Yes";
        }
        else
        {
            per = "No";
        }
        sb.Append('\t' + per);
    }
    sb.Append(Environment.NewLine);
}

System.IO.File.WriteAllText(folderName + @"\Perspectives.tsv", sb.ToString());

/**********************************************************************/
/*************************CALCULATION GROUPS***************************/

sb = new System.Text.StringBuilder();

// Headers
sb.Append("CalculationGroup" + '\t' + "CalculationItem" + '\t' + "Expression" + '\t' + "Ordinal" + '\t' + "FormatString" + '\t' + "Description");
sb.Append(Environment.NewLine);

foreach (var o in Model.CalculationGroups.ToList())
{
    foreach (var i in o.CalculationItems.ToList())
    {
        string cg = o.Name;
        string ci = i.Name;
        string expr = i.Expression;
        
        // Remove tabs and new lines
        expr = expr.Replace("\n"," ");
        expr = expr.Replace("\t"," ");

        string ord = i.Ordinal.ToString();
        string fs = i.FormatStringExpression;
        string desc = i.Description;
        
        sb.Append(cg + '\t' + ci + '\t' + expr + '\t' + ord + '\t' + fs + '\t' + desc);
        sb.Append(Environment.NewLine);
    }
}

System.IO.File.WriteAllText(folderName + @"\CalculationGroups.tsv", sb.ToString());

/**********************************************************************/
/*************************PARTITIONS***************************/

sb = new System.Text.StringBuilder();

sb.Append("tableName" + '\t' + "partitionName" + '\t' + "Expressions" + '\t' + "Mode" + '\t' + "SourceType");
sb.Append(Environment.NewLine);

foreach (var p in Model.AllPartitions.ToList())
{
    string tableName = p.Table.Name;
    string partitionName = p.Name;
    string expr = p.Expression;
    var mode = p.Mode;
    var SourceType = p.SourceType;
    
    if (!String.IsNullOrEmpty(expr))
    {
        // Remove tabs and new lines
        expr = expr.Replace("\n"," ");
        expr = expr.Replace("\t"," ");
    }
    sb.Append(tableName +'\t' + partitionName + '\t' + expr +'\t' + mode + '\t' + SourceType);
    sb.Append(Environment.NewLine);    
}

System.IO.File.WriteAllText(folderName + @"\partitions.tsv", sb.ToString());

