using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GenCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StringBuilder sb = new StringBuilder();
        StringBuilder colorSb = new StringBuilder();
        int order = 1;
        int colorMask = 0;
        for (int i = 1; i <= 32; i++)
        {
            colorMask = 4 + 8 * (i - 1);
            sb.Append(GetStrCode(i, order, "group" + i, colorMask));
            order += 2;
            sb.Append("\n");

            //float color = colorMask * 1.0f / 255;
            //colorSb.Append($"C{i} = new Color({color}f, {color}f, {color}f, {color}f);\n");
        }
        Debug.Log(sb.ToString());
        //Debug.Log(colorSb.ToString());
    }


    public string GetStrCode(int i, int order, string group, int colorMask)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"[PropertyOrder({i})]\n");
        string labelTxt = GetIntStr(colorMask) + "_C" + GetIntStr1(i);
        sb.Append($"[LabelText(\"{labelTxt}\")][LabelWidth(60)]\n");
        sb.Append($"public Gradient C{i};\n");

        return sb.ToString();
    }

    public string GetStrCode2(int i, int order, string group, int colorMask)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"[PropertyOrder({order})]\n");
        sb.Append($"[HorizontalGroup(\"{group}\", width: 20)]\n");
        sb.Append($"[HideLabel][ReadOnly]\n");
        sb.Append($"public Color C{i};\n");

        sb.Append("\n");

        sb.Append($"[PropertyOrder({order + 1})]\n");
        sb.Append($"[HorizontalGroup(\"{group}\", LabelWidth = 52)]\n");
        string labelTxt = GetIntStr(colorMask) + "_C" + GetIntStr1(i);
        sb.Append($"[LabelText(\"{labelTxt}\")]\n");
        sb.Append($"public Gradient GradC{i};\n");

        return sb.ToString();
    }

    public string GetStrCode1(int i, int order, string group, int colorMask)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"[PropertyOrder({order})]\n");
        sb.Append($"[HorizontalGroup(\"{group}\", width: 20)]\n");
        float color = colorMask*1.0f / 255;
        sb.Append($"[GUIColor({colorMask}f, {colorMask}f, {colorMask}f)]\n");
        sb.Append($"[Button(\" \")]\n");
        sb.Append($"private void OnBtn{i}" + "() { }\n");

        sb.Append("\n");

        sb.Append($"[PropertyOrder({order+1})]\n");
        sb.Append($"[HorizontalGroup(\"{group}\", LabelWidth = 52)]\n");
        string labelTxt = GetIntStr(colorMask) + "_C" + GetIntStr1(i);
        sb.Append($"[LabelText(\"{labelTxt}\")]\n");
        sb.Append($"public Gradient C{i};\n");

        return sb.ToString();
    }

    public string GetIntStr(int num)
    {
        if (num < 10)
            return "00" + num;
        else if (num < 100)
            return "0" + num;
        return num.ToString();
    }

    public string GetIntStr1(int num)
    {
        if (num < 10)
            return "0" + num;
        return num.ToString();
    }

}
