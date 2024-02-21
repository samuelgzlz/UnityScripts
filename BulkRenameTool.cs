using UnityEngine;
using UnityEditor;

public class BulkRenameTool : EditorWindow
{
    private string part1 = "";
    private string part2 = "";
    private string part3 = "";
    private string part4 = "";
    private int startNumber = 1;
    private bool useLetters = false;
    private bool addSpaces = true;
    private int numberOfDigits = 3; 

    [MenuItem("Window/Bulk Rename Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BulkRenameTool));
    }

    void OnGUI()
    {
        GUILayout.Label("Bulk Rename Settings", EditorStyles.boldLabel);
        part1 = EditorGUILayout.TextField("Part 1 (e.g., Spot Light)", part1);
        part2 = EditorGUILayout.TextField("Part 2 (e.g., Top/A)", part2);
        part3 = EditorGUILayout.TextField("Part 3 (e.g., Blue)", part3);
        part4 = EditorGUILayout.TextField("Part 4 (e.g., Strobe)", part4);

        useLetters = EditorGUILayout.Toggle("Use Letters", useLetters);
        addSpaces = EditorGUILayout.Toggle("Space Between Words", addSpaces);
        startNumber = EditorGUILayout.IntField("Start Number/Letter", startNumber);
        numberOfDigits = EditorGUILayout.IntField("Number of Digits/Letters", numberOfDigits);

        if (GUILayout.Button("Rename Selected Objects"))
        {
            RenameObjects();
        }
    }

    void RenameObjects()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("BulkRenameTool: No objects selected.");
            return;
        }

        int count = startNumber;
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj, "Bulk Rename");
            string newName = ConstructName(part1, part2, part3, part4, useLetters, count, numberOfDigits);
            obj.name = newName;
            count++;
        }
    }

    string ConstructName(string p1, string p2, string p3, string p4, bool letters, int count, int digits)
    {
        //Stringbuilder for performance
        System.Text.StringBuilder newName = new System.Text.StringBuilder();
        string delimiter = addSpaces ? " " : "";

        //Remove any leading/trailing spaces
        AppendPart(newName, p1, delimiter);
        AppendPart(newName, p2, delimiter);
        AppendPart(newName, p3, delimiter);
        AppendPart(newName, p4, delimiter);

        //Characer increment
        string incrementalPart = letters ? GetLetterSequence(count, digits) : count.ToString($"D{digits}");
        if (!string.IsNullOrEmpty(incrementalPart))
        {
            if (newName.Length > 0 && addSpaces) // Check if we need to prepend a delimiter
                newName.Append(delimiter);
            newName.Append(incrementalPart);
        }

        return newName.ToString();
    }

    void AppendPart(System.Text.StringBuilder builder, string part, string delimiter)
    {
        if (!string.IsNullOrEmpty(part))
        {
            if (builder.Length > 0 && !string.IsNullOrEmpty(delimiter))
                builder.Append(delimiter);
            builder.Append(part);
        }
    }

    string GetLetterSequence(int count, int length)
    {
        string result = "";
        count--; // Adjust count to start from 0.

        for (int i = 0; i < length; i++)
        {
            if (count < 0)
            {
                result = 'A' + result;
                continue;
            }

            int charIndex = count % 26; // Find the position in the alphabet.
            char letter = (char)('A' + charIndex);
            result = letter + result; // Build the string in reverse order for correct sequencing.
            count = count / 26 - 1; // Move to the next digit.
        }
        return result;
    }
}