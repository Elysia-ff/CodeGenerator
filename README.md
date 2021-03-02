# CodeGenerator
Creates a text file from given template for Unity

![image](https://user-images.githubusercontent.com/45890606/109596326-5f883580-7b59-11eb-8880-c2b453b98346.png)

![image](https://user-images.githubusercontent.com/45890606/109595238-7af24100-7b57-11eb-87af-b0da006b4d8f.png)  
1. Assign your template.  
2. Click it to open the template with text editor (maybe VS or notepad).
3. The variables that the template has. Click their names to see their locations in the template.
4. The variables will be replaced with these.
5. File name. If extension isn't specified, will be .cs.
6. Path of the file. By Selecting a folder or file in Project view, you can change it.
7. Done!

Result ..
```cs
using UnityEngine;

public class TestName : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void DoSomething()
    {
        Debug.Log("DoSomething");
    }
}
```

## Make your own template
Create any kind of text file. .txt is will be the easiest one.  
Place `%%` before and after your variables. (see [sample template](https://github.com/Elysia-ff/CodeGenerator/blob/main/Assets/CodeGenerator/Editor/sample%20template.txt))  
Only alphabets, numbers, underscore and a space allowd! [regex](https://github.com/Elysia-ff/CodeGenerator/blob/f62877d7f1c67e91c05549d370f4361a228a1502/Assets/CodeGenerator/Editor/CodeGenerator.cs#L12)
