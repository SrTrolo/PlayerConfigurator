using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class AppControl : MonoBehaviour
{
    public List<GameObject> allsounds;
    public GameObject Camera, PAUSE;
    public List<Transform> CameraPosT, CameraPosO;

    private int currentPos;
    private categoryType currentCategory;
    public enum categoryType { TEXTURES, ACCESSORIES};
    
    public Transform gridCategories, gridSubCategories;
    public GameObject baseBtn;

    public List<Transform> bones;
    public string keySave;
    public List<string> allKeySave;

    public class SaveParameters
    {
        public int[] textures, accessories;

        public SaveParameters() { }
        public SaveParameters(int[] _texture, int[] _accessories)
        {
            textures = _texture;
            accessories = _accessories;
        }
    }
    public SaveParameters saves;
    private int[] textures = new int[8];
    private int[] accessories = new int[4];

    void Start()
    {

        PAUSE.SetActive(false);
        //PlayerPrefs.DeleteAll();
        allKeySave = PlayerPrefsX.GetStringList("AllSaves");

        for (int i = 0; i < accessories.Length; i++)
        {
            accessories[i] = -1;
        }
        CameraControl(categoryType.TEXTURES, 0);
        PrintCategories();
    }
    void PrintCategories()
    {
        int totalCategories = Resources.LoadAll<Sprite>("Categories").Length;
        for (int i = 0; i < totalCategories; i++)
        {
            GameObject newCategory = Instantiate(baseBtn, gridCategories);
            newCategory.transform.GetChild(0).GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Categories/" + i);

            int tempIndex = i;
            newCategory.GetComponent<Button>().onClick.AddListener(
                delegate { PrintElements(categoryType.TEXTURES, tempIndex); });
        }

        int totalCatAccesories = Resources.LoadAll<Sprite>("CatAccessories").Length;
        for (int i = 0; i < totalCatAccesories; i++)
        {
            GameObject newCategory = Instantiate(baseBtn, gridCategories);
            newCategory.transform.GetChild(0).GetComponent<Image>().sprite =
                Resources.Load<Sprite>("CatAccessories/" + i);

            int tempIndex = i;
            newCategory.GetComponent<Button>().onClick.AddListener(
                delegate { PrintElements(categoryType.ACCESSORIES, tempIndex); });
        }

    }
    
    void PrintElements(categoryType _type, int _index)
    {
        
        for (int i = gridSubCategories.childCount - 1; i >= 0; i--)
        {
            Destroy(gridSubCategories.GetChild(i).gameObject);
        }
        switch (_type)
        {
            case categoryType.TEXTURES:
                GameObject newSound = Instantiate(allsounds[0]);
                Destroy(newSound, 1);
                CameraControl(_type, _index);
                int texturesCount = Resources.LoadAll<Sprite>("Elements/" + _index).Length;
                for (int i = 0; i < texturesCount; i++)
                {
                    GameObject newTexture = Instantiate(baseBtn, gridSubCategories);
                    newTexture.transform.GetChild(0).GetComponent<Image>().sprite = 
                        Resources.Load<Sprite>("Elements/" + _index + "/" + i);

                    int category = _index;
                    int subCategory = i;
                    newTexture.GetComponent<Button>().onClick.AddListener(
                        delegate { setTexture(category, subCategory); });

                }

                break;
            case categoryType.ACCESSORIES:
                GameObject newSound1 = Instantiate(allsounds[0]);
                Destroy(newSound1, 1);
                CameraControl(_type, _index);
                int accessoriesCount = Resources.LoadAll<Sprite>("ElemAccessories/" + _index).Length;
                GameObject emptyElement = Instantiate(baseBtn, gridSubCategories);
                emptyElement.transform.GetChild(0).GetComponent<Image>().enabled = false;
                emptyElement.GetComponent<Button>().onClick.AddListener(
                        delegate { setAccessory(_index, -1); });
                emptyElement.transform.SetSiblingIndex(0); //Cambiar la posicion del boton

                for (int i = 0; i < accessoriesCount; i++)
                {
                    GameObject newAccessorie = Instantiate(baseBtn, gridSubCategories);
                    newAccessorie.transform.GetChild(0).GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("ElemAccessories/" + _index + "/" + i);

                    int category = _index;
                    int subCategory = i;
                    newAccessorie.GetComponent<Button>().onClick.AddListener(
                        delegate { setAccessory(category, subCategory); });
                }
                break;

        }
    }
    void setTexture(int category, int subcategory)
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);

        textures[category] = subcategory;
        Material mat = Resources.Load<Material>("Materials/" + category);
        Texture2D albedo = Resources.Load<Texture2D>("Textures/" + subcategory);
        Texture2D normal = Resources.Load<Texture2D>("Textures/" + subcategory + "/Normal");

        mat.SetTexture("_MainTex", albedo);
        mat.SetTexture("_BumpMap", normal);
    }

    void setAccessory(int category, int subcategory)
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);

        accessories[category] = subcategory;

        for (int i = bones[category].childCount - 1; i >= 0; i--)
        {
            Destroy(bones[category].GetChild(i).gameObject);
        }

        GameObject newAsset = Resources.Load<GameObject>("Accessories/" + category + "/" + subcategory);
        if(newAsset != null)
        {
            newAsset = Instantiate(newAsset, bones[category]);
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            
            PauseControl();
        }
        
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    SaveSkin();
        //}
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    LoadSkin();
        //}
        //if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    DeleteSave();
        //}

        UpdateCamera();
    }
    public void SaveSkin()
    {
        DeleteSave();
        if (allKeySave.Contains(keySave) == false)
        {
            allKeySave.Add(keySave);
        }
        
        PlayerPrefsX.SetStringList("AllSaves", allKeySave);
        SaveParameters newSave = new SaveParameters(textures, accessories);
        XmlSerializer serial = new XmlSerializer(typeof(SaveParameters));
        using(StringWriter writer = new StringWriter())
        {
            serial.Serialize(writer, newSave);
            PlayerPrefs.SetString(keySave, writer.ToString());
        }
    }
    public void LoadSkin()
    {
        string newLoad = PlayerPrefs.GetString(keySave);
        if(newLoad.Length>0)
        {
            XmlSerializer serial = new XmlSerializer(typeof(SaveParameters));
            using (StringReader reader = new StringReader(newLoad))
            {
                SaveParameters load = serial.Deserialize(reader) as SaveParameters;
                textures = load.textures;
                accessories = load.accessories;
            }

            for (int i = 0; i < textures.Length; i++)
            {
                setTexture(i, textures[i]);
            }
            for (int i = 0; i < accessories.Length; i++)
            {
                setAccessory(i, accessories[i]);
            }
        }
        
    }
    void DeleteSave()
    {
        allKeySave.Remove(keySave);
        PlayerPrefsX.SetStringList("AllSaves", allKeySave);
        PlayerPrefs.DeleteKey(keySave);
    }

    void UpdateCamera()
    {
        if(currentCategory == categoryType.TEXTURES)
        {
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraPosT[currentPos].transform.position, 3 * Time.deltaTime);
        }
        if (currentCategory == categoryType.ACCESSORIES)
        {
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraPosO[currentPos].transform.position, 3 * Time.deltaTime);
        }



    }
    void CameraControl(categoryType _type, int _btnId)
    {
        switch (_type)
        {
            case categoryType.TEXTURES:
                currentPos = _btnId;
                currentCategory = _type;
                break;
            case categoryType.ACCESSORIES:
                currentPos = _btnId;
                currentCategory = _type;
                break;
        }
        
    }
    public void PauseControl()
    {

        PAUSE.SetActive(true);
    }
    public void GoMenu()
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);
        SceneManager.LoadScene(0);
    }
    public void CancelMenu()
    {
        GameObject newSound = Instantiate(allsounds[0]);
        Destroy(newSound, 1);
        PAUSE.SetActive(false);
    }

}
