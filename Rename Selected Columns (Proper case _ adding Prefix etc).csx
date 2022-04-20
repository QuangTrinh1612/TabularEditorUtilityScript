foreach(var obj in Selected.Columns) {
    var oldName = obj.Name.Replace('_',' ');
    var newName = new System.Text.StringBuilder();
    for(int i = 0; i < oldName.Length; i++) {
        // First letter should always be capitalized:
        if(i == 0) newName.Append(Char.ToUpper(oldName[i]));

        // To change id to ID
        else if(i+1 < oldName.Length && oldName[i].Equals(' ') && oldName[i+1].Equals('i')&& oldName[i+2].Equals('d'))
        {
            newName.Append(oldName[i]);
            newName.Append(Char.ToUpper(oldName[i+1]));
            newName.Append(Char.ToUpper(oldName[i+2]));
            i=i+2;
        }
        // To change first letter after Space to Upper:
        else if(i+1 < oldName.Length && oldName[i].Equals(' '))
        {
            newName.Append(oldName[i]);
            newName.Append(Char.ToUpper(oldName[i+1]));
            i=i+1;
        }
        else
        {
            newName.Append(oldName[i]);
        }
    }
    // To add prefix Base:
    // obj.Name = "Base: " + newName.ToString();
    // without prefix Base:
    obj.Name = newName.ToString();
}