using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class RampGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [LabelText("开启调色板")]
    public bool IsDebug = false;

    [ValidateInput("MatsValidate", "未添加材质球", InfoMessageType.Error)]
    public List<Material> Mats;
    private bool MatsValidate() { return Mats != null && Mats.Count > 0; }

    #region propeties
    [PropertyOrder(1)]
    [LabelText("004_C01")]
    [LabelWidth(60)]
    public Gradient C1;

    [PropertyOrder(2)]
    [LabelText("012_C02")]
    [LabelWidth(60)]
    public Gradient C2;

    [PropertyOrder(3)]
    [LabelText("020_C03")]
    [LabelWidth(60)]
    public Gradient C3;

    [PropertyOrder(4)]
    [LabelText("028_C04")]
    [LabelWidth(60)]
    public Gradient C4;

    [PropertyOrder(5)]
    [LabelText("036_C05")]
    [LabelWidth(60)]
    public Gradient C5;

    [PropertyOrder(6)]
    [LabelText("044_C06")]
    [LabelWidth(60)]
    public Gradient C6;

    [PropertyOrder(7)]
    [LabelText("052_C07")]
    [LabelWidth(60)]
    public Gradient C7;

    [PropertyOrder(8)]
    [LabelText("060_C08")]
    [LabelWidth(60)]
    public Gradient C8;

    [PropertyOrder(9)]
    [LabelText("068_C09")]
    [LabelWidth(60)]
    public Gradient C9;

    [PropertyOrder(10)]
    [LabelText("076_C10")]
    [LabelWidth(60)]
    public Gradient C10;

    [PropertyOrder(11)]
    [LabelText("084_C11")]
    [LabelWidth(60)]
    public Gradient C11;

    [PropertyOrder(12)]
    [LabelText("092_C12")]
    [LabelWidth(60)]
    public Gradient C12;

    [PropertyOrder(13)]
    [LabelText("100_C13")]
    [LabelWidth(60)]
    public Gradient C13;

    [PropertyOrder(14)]
    [LabelText("108_C14")]
    [LabelWidth(60)]
    public Gradient C14;

    [PropertyOrder(15)]
    [LabelText("116_C15")]
    [LabelWidth(60)]
    public Gradient C15;

    [PropertyOrder(16)]
    [LabelText("124_C16")]
    [LabelWidth(60)]
    public Gradient C16;

    [PropertyOrder(17)]
    [LabelText("132_C17")]
    [LabelWidth(60)]
    public Gradient C17;

    [PropertyOrder(18)]
    [LabelText("140_C18")]
    [LabelWidth(60)]
    public Gradient C18;

    [PropertyOrder(19)]
    [LabelText("148_C19")]
    [LabelWidth(60)]
    public Gradient C19;

    [PropertyOrder(20)]
    [LabelText("156_C20")]
    [LabelWidth(60)]
    public Gradient C20;

    [PropertyOrder(21)]
    [LabelText("164_C21")]
    [LabelWidth(60)]
    public Gradient C21;

    [PropertyOrder(22)]
    [LabelText("172_C22")]
    [LabelWidth(60)]
    public Gradient C22;

    [PropertyOrder(23)]
    [LabelText("180_C23")]
    [LabelWidth(60)]
    public Gradient C23;

    [PropertyOrder(24)]
    [LabelText("188_C24")]
    [LabelWidth(60)]
    public Gradient C24;

    [PropertyOrder(25)]
    [LabelText("196_C25")]
    [LabelWidth(60)]
    public Gradient C25;

    [PropertyOrder(26)]
    [LabelText("204_C26")]
    [LabelWidth(60)]
    public Gradient C26;

    [PropertyOrder(27)]
    [LabelText("212_C27")]
    [LabelWidth(60)]
    public Gradient C27;

    [PropertyOrder(28)]
    [LabelText("220_C28")]
    [LabelWidth(60)]
    public Gradient C28;

    [PropertyOrder(29)]
    [LabelText("228_C29")]
    [LabelWidth(60)]
    public Gradient C29;

    [PropertyOrder(30)]
    [LabelText("236_C30")]
    [LabelWidth(60)]
    public Gradient C30;

    [PropertyOrder(31)]
    [LabelText("244_C31")]
    [LabelWidth(60)]
    public Gradient C31;

    [PropertyOrder(32)]
    [LabelText("252_C32")]
    [LabelWidth(60)]
    public Gradient C32;
    #endregion

    #region ReadTex
    [HorizontalGroup("Group0", width:0.4f)]
    [HideLabel]
    public Texture2D ReadFromTex;
    [HorizontalGroup("Group0")]
    [Button("读取纹理")]
    [GUIColor(0, 1, 0)]
    private void ReadColorFromTex()
    {
        if (ReadFromTex == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择图片", "确认");
            return;
        }
        if (ReadFromTex.isReadable)
        {
            List<List<GradientColorKey>> colorBands = new List<List<GradientColorKey>>();
            for (int j = 0; j < 32; j++)
            {
                List<GradientColorKey> colorKeys = new List<GradientColorKey>();
                Color last = Color.white;
                last.a = 0;
                int y = 256 - (4 + 8 * j);
                for (int i = 0; i < 256; i++)
                {
                    Color c = ReadFromTex.GetPixel(i, y);
                    if (c != last)
                    {
                        if (colorKeys.Count > 0)
                        {
                            GradientColorKey ckEnd = new GradientColorKey();
                            ckEnd.color = last;
                            ckEnd.time = (float)i / 255;
                            colorKeys.Add(ckEnd);
                        }
                        GradientColorKey ckNew = new GradientColorKey();
                        ckNew.color = c;
                        ckNew.time = (float)i / 255;
                        colorKeys.Add(ckNew);
                        last = c;
                    }
                    else
                    {
                        if (i == 255)
                        {
                            GradientColorKey endCk = new GradientColorKey();
                            endCk.color = c;
                            endCk.time = 1;
                            colorKeys.Add(endCk);
                        }
                    }
                }
                colorBands.Add(colorKeys);
            }
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;
            for (int n = 0; n < 32; n++)
            {
                Gradient tempColor = GetColor(n);
                tempColor.mode = GradientMode.Fixed;
                GradientColorKey[] colorKey = MergeColorKey(colorBands[n]);
                if (colorKey.Length <= 8)
                {
                    tempColor.SetKeys(colorKey, alphaKey);
                }
                else
                {
                    Debug.Log($"第{n}条超过了8,条数{colorKey.Length}");
                    for (int i = 0; i < colorKey.Length; i++)
                    {
                        Debug.Log(colorKey[i].color + "_" + colorKey[i].time);
                    }
                }
            }
            OnValidate();
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "请将纹理设置为可读", "确认");
        }
    }

    private GradientColorKey[] MergeColorKey(List<GradientColorKey> colorKeys)
    {
        List<GradientColorKey> res = new List<GradientColorKey>();
        float del = 0.02f;
        GradientColorKey last = new GradientColorKey(Color.black, -1);
        for (int i = 0; i < colorKeys.Count; i++)
        {
            GradientColorKey temp = colorKeys[i];
            if (i == 0)
            {
                last = temp;
                res.Add(temp);
                continue;
            }
            if (temp.time - last.time < del)
            {
                last = temp;
            }
            else
            {
                last = temp;
                res.Add(temp);
            }
        }
        return res.ToArray();
    }
    #endregion

    #region GenTex

    [HorizontalGroup("Group1", width: 0.4f)]
    [HideLabel][ReadOnly]
    public Texture2D RampTexture;
    [HorizontalGroup("Group1")]
    [Button("生成纹理")]
    [GUIColor(0, 1, 0)]
    private void GenTex()
    {
        string path = EditorUtility.SaveFilePanel("保持纹理", "", "RampTex", "png");
        if (!string.IsNullOrEmpty(path))
        {
            var bytes = RampTexture.EncodeToPNG();
            if (bytes != null && bytes.Length > 0)
            {
                System.IO.File.WriteAllBytes(path, RampTexture.EncodeToPNG());
                AssetDatabase.Refresh();
            }
        }
    }
    private readonly int size = 256;
    void OnValidate()
    {
        if (IsDebug)
        {
            Init();
            DoIt();
        }
    }


    public void DoIt()
    {
        Gradient temp = GetColor(0);
        int i = 31;
        //让色带从上往下排
        for (int y = 0; y < 256; y++)
        {
            //每条高8个像素
            if (y % 8 == 0)
            {
                temp = GetColor(i);
                i--;
            }
            if (RampTexture != null && temp != null)
            {
                for (int x = 0; x < size; x++)
                {
                    RampTexture.SetPixel(x, y, temp.Evaluate((float)x / size));
                }
            }
        }

        if (RampTexture != null)
            RampTexture.Apply();

        //全局赋值
        //Shader.SetGlobalTexture("_RampTex", RampTexture);
        //Mat.SetTexture(Shader.PropertyToID("_RampTex'"), RampTexture);
        if (Mats != null && Mats.Count > 0)
        {
            for (int j = 0; j < Mats.Count; j++)
            {
                if(Mats[j] != null)
                    Mats[j].SetTexture("_RampTex", RampTexture);
            }
        }
    }

    private void Init()
    {
        if (RampTexture == null)
        {
            RampTexture = new Texture2D(size, size, TextureFormat.RGB24, false, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
        }
    }

    public Gradient GetColor(int i)
    {
        switch (i)
        {
            case 0: return C1;
            case 1: return C2;
            case 2: return C3;
            case 3: return C4;
            case 4: return C5;
            case 5: return C6;
            case 6: return C7;
            case 7: return C8;
            case 8: return C9;
            case 9: return C10;
            case 10: return C11;
            case 11: return C12;
            case 12: return C13;
            case 13: return C14;
            case 14: return C15;
            case 15: return C16;
            case 16: return C17;
            case 17: return C18;
            case 18: return C19;
            case 19: return C20;
            case 20: return C21;
            case 21: return C22;
            case 22: return C23;
            case 23: return C24;
            case 24: return C25;
            case 25: return C26;
            case 26: return C27;
            case 27: return C28;
            case 28: return C29;
            case 29: return C30;
            case 30: return C31;
            case 31: return C32;
            default: return C1;
        }
    }
    #endregion
#endif
}
