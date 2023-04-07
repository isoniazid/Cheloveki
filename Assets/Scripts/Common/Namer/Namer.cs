/*
Namer: a lib for generating japanese names, toponyms etc.
Vasya Ryzhkoff, 2023
*/


using System.Collections.Generic;
using Newtonsoft.Json;
using Ieroglyphs;
public static class Namer
{

    private static string _jsonFile = "Assets/Scripts/Common/Namer/ieroglyph_list.json";
    private static string _jsonText = System.IO.File.ReadAllText(_jsonFile);
    private static List<Ieroglyph> _ieroglyphs = JsonConvert.DeserializeObject<List<Ieroglyph>>(_jsonText);

    private static bool NextIsVowel(string name, int index)
    {
        char[] vowels = { 'а', 'и', 'у', 'э', 'о', 'я', 'ю', 'ё' };
        foreach (var vovel in vowels)
        {
            if (name[index + 1] == vovel) return true;
        }
        return false;
    }

    public static (string name, string meaning) MakeName(int max_len)
    {
        if (_ieroglyphs == null) throw (new System.NullReferenceException($"Json data was not found. Check {_jsonFile}"));

        var rndGen = new System.Random();
        int nameLen = rndGen.Next(2, max_len);
        int ieroglyphArrayLen = _ieroglyphs.Count;

        string name = "";
        string meaning = "";


        for (int i = 0; i < nameLen; ++i)
        {
            Ieroglyph currentIeroglyph = _ieroglyphs[rndGen.Next(ieroglyphArrayLen)];
            int spellingArrayLen = currentIeroglyph.spelling.Count; //У одного иероглифа есть разные варианты произношения

            meaning += currentIeroglyph.meaning + " ";

            name += currentIeroglyph.spelling[rndGen.Next(spellingArrayLen)];
        }


        var sokuonIndex = name.IndexOf('@');
        if (sokuonIndex != -1)
        {
            //Если сокуон не в конце слова и не перед гласной....
            if (sokuonIndex != name.Length - 1 && !NextIsVowel(name, sokuonIndex)) name = name.Replace('@', name[sokuonIndex + 1]);
            else return MakeName(max_len); //Иначе генери новое слово...
        }

        name = name.Substring(0,1).ToUpper() + name.Substring(1);

        return (meaning, name);
    }

}