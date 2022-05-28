using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SettingControll : MonoBehaviour
{
    public GUIStyle boxtexture, button, selectedbutton,paperbox,textbox,textfilebox;
    private int guisection = 0, selectedselection = 0;
    private List<string> arbitraryselections;
    private string bufferstring;
    public List<int> bufferindex, selectedcardindex;
    private bool addsection, addcard,addimage,creategreatpoint,addimportantevent,leftmenu,showinfo,showhelp,movenow,issave;
    private int addtype;
    private List<Card> cards;
    private int megacounter = 0, cardselector = 0,scroller=0;
    public Texture2D dragpic, scalepic,verticalscalepic,horizontalscalepic,trashpic,sandclockpic,SettingControlLogo,dicepic,scrollpic,movepic,eyepic,savepic;
    //фаза интерфейса
    private int guiphase = 0;
    //загрузка/выгрузка
    private string projectpath;
    //список проектов
    private List<string> projectlist,datelist;
    //список текстовиков
    private List<string> textslist;
    private List<string> stringlist;
    private int linecounter =0;
    private int textselector = -1;
    //таймлайн
    private Texture2D graytexture;
    private int timeselector = -1;
    private int timezoneselector = -1;
    private bool timezonetype = false;
    private bool timelinegroup = false;
    //настройки таймлайна
    private int timescaler = 1;
    private string timenamer = "Год";
    //ключевые точки
    private Texture2D greentexture,litegreentexture,orangetexture;
    private string greatpointname = "";
    private int greatpointpositionselector = 0;
    //важные события
    private List<ImportantEvent> importantEvents;
    private Color buffercolor;
    private Texture2D buffercolortexture;
    private int buffertimepos;
    //карты
    private List<string> maps;
    private Texture2D selectedmap;
    private int mapselector = -1;
    //зоны
    private List<MapZonesList> mapszones;
    private int selectedzone = -1;
    private bool zonemode, zoneadd,zoneeditmode,pointeditmode;
    //указатель
    private Vector2Int point;
    private bool rightselector = false;
    //перетаскивание и растягивание
    private int currentdrag;
    private bool drag = false;
    private int scaletype = 0;
    //шаблоны карточек
    private List<string> presetlist;
    private bool showpresetlist = false;
    private bool createpreset = false;
    //приколюхи
    private string projectname;
    private char[] chars = {'а','ы', 'у', 'р', 'в', 'ф', 'г', 'л', 'й', 'ц' };
    private string[] timenamers = { "День", "Сол", "Неделя", "Месяц", "Сезон", "Год","Декада", "Век", "Тысячилетие"};
    private int savetimer = 3000;
    //улучшенная работа с картой
    private float scale =1f;
    private Vector2 shift = new Vector2();
    private void InitializeProject()
    {
        arbitraryselections = new List<string>();
        arbitraryselections.Add("Места");
        arbitraryselections.Add("Культуры");
        arbitraryselections.Add("Персонажи");
        cards = new List<Card>();
        bufferindex = new List<int>();
        selectedcardindex = new List<int>();
        cards.Add(new Card("Места", new List<int>()));
        cards[0].index.Add(0);
        cards.Add(new Card("Культуры", new List<int>()));
        cards[1].index.Add(1);
        cards.Add(new Card("Персонажи", new List<int>()));
        cards[2].index.Add(2);
        selectedcardindex.Add(0);
    }
    private void CreateProject() {
        Directory.CreateDirectory(Application.dataPath + "/projects/"+bufferstring);
        projectpath = Application.dataPath + "/projects/" + bufferstring;
        StreamWriter file = new StreamWriter(projectpath+"/project.secod");
        Directory.CreateDirectory(projectpath + "/maps");
        Directory.CreateDirectory(projectpath + "/pictures");
        file.Close();
    }
    private void SaveCard(Card card,StreamWriter file) {
        file.WriteLine(card.name);
        file.WriteLine(card.index.Count);
        for (int i = 0; i < card.index.Count; i++) {
            file.WriteLine(card.index[i]);
        }
        file.WriteLine(card.blocks.Count);
        for (int i = 0; i < card.blocks.Count; i++) {
            file.WriteLine(card.blocks[i].image);
            file.WriteLine(card.blocks[i].note);
            file.WriteLine(card.blocks[i].rect.x);
            file.WriteLine(card.blocks[i].rect.y);
            file.WriteLine(card.blocks[i].rect.width);
            file.WriteLine(card.blocks[i].rect.height);
            file.WriteLine(card.blocks[i].text);
            file.WriteLine(card.blocks[i].time);
            file.WriteLine(card.blocks[i].timegroup);
            file.WriteLine(card.blocks[i].timezonetype);
        }
        file.WriteLine(card.childrens.Count);
        if (card.childrens.Count > 0) {
            for (int i = 0; i < card.childrens.Count; i++) {
                SaveCard(card.childrens[i], file);
            }
        }
    }
    private Card LoadCard(StreamReader file) {
        string buffername = file.ReadLine();
        int bufferlenght = int.Parse(file.ReadLine());
        int[] bufferindex = new int[bufferlenght];
        for (int i = 0; i < bufferlenght; i++) { bufferindex[i] = int.Parse(file.ReadLine());}
        Card RetCard = new Card(buffername,new List<int>(bufferindex));
        bufferlenght = int.Parse(file.ReadLine());
        for (int i = 0; i < bufferlenght; i++) {
            bool image = bool.Parse(file.ReadLine());
            bool note = bool.Parse(file.ReadLine());
            Rect rect = new Rect();
            rect.x = float.Parse(file.ReadLine());
            rect.y = float.Parse(file.ReadLine());
            rect.width = float.Parse(file.ReadLine());
            rect.height = float.Parse(file.ReadLine());
            buffername = file.ReadLine();
            int bufferint = int.Parse(file.ReadLine());
            bool bool0 = bool.Parse(file.ReadLine());
            bool bool1 = bool.Parse(file.ReadLine());
            Block block = new Block(rect,buffername);
            block.image = image;
            block.note = note;
            block.time = bufferint;
            block.timegroup = bool0;
            block.timezonetype = bool1;
            RetCard.blocks.Add(block);
                }
        bufferlenght = int.Parse(file.ReadLine());
        for (int i = 0; i < bufferlenght; i++) {
            RetCard.childrens.Add(LoadCard(file));
        }
        return RetCard;
    }
    private void SaveProject() {
        StreamWriter file = new StreamWriter(projectpath+"/project.secod");
        file.WriteLine(arbitraryselections.Count);
        for (int i = 0; i < arbitraryselections.Count; i++) {
            file.WriteLine(arbitraryselections[i]);
        }
        file.WriteLine(cards.Count);
        for (int i = 0; i < cards.Count; i++) {
            SaveCard(cards[i], file);
        }
        file.WriteLine(greatpointname);
        file.WriteLine(greatpointpositionselector);
        file.WriteLine(timenamer);
        file.WriteLine(timescaler);
        file.WriteLine(importantEvents.Count);
        for (int i = 0; i < importantEvents.Count; i++) {
            file.WriteLine(importantEvents[i].name);
            file.WriteLine(importantEvents[i].position);
            file.WriteLine(importantEvents[i].color.r);
            file.WriteLine(importantEvents[i].color.g);
            file.WriteLine(importantEvents[i].color.b);
        }
        file.Close();
        for (int i = 0; i < maps.Count; i++)
        {
            bufferstring = "";
            for (int ii = 0; ii < maps[i].Length - 4; ii++)
            {
                bufferstring += maps[i][ii];
            }
            StreamWriter file0 = new StreamWriter(projectpath + "/maps/" + bufferstring + ".secomd");
            file0.WriteLine(mapszones[i].name);
            file0.WriteLine(mapszones[i].mapZones.Count);
            for (int ii = 0; ii < mapszones[i].mapZones.Count;ii++) {
                file0.WriteLine(mapszones[i].mapZones[ii].name);
                file0.WriteLine(mapszones[i].mapZones[ii].color.r);
                file0.WriteLine(mapszones[i].mapZones[ii].color.g);
                file0.WriteLine(mapszones[i].mapZones[ii].color.b);
                file0.WriteLine(mapszones[i].mapZones[ii].points.Count);
                for (int iii = 0; iii < mapszones[i].mapZones[ii].points.Count; iii++) {
                    file0.WriteLine(mapszones[i].mapZones[ii].points[iii].x);
                    file0.WriteLine(mapszones[i].mapZones[ii].points[iii].y);
                }
            }
            file0.Close();
        }
        issave = true;
        savetimer = 3000;
    }
    private void LoadProject() {
        arbitraryselections = new List<string>();
        StreamReader file = new StreamReader(projectpath+"/project.secod");
        int bufferint = int.Parse(file.ReadLine());
        for (int i = 0; i < bufferint; i++) {
            arbitraryselections.Add(file.ReadLine());
        }
        bufferint = int.Parse(file.ReadLine());
        cards = new List<Card>();
        for (int i = 0; i < bufferint; i++) {
            cards.Add(LoadCard(file));
        }
        greatpointname = file.ReadLine();
        greatpointpositionselector = int.Parse(file.ReadLine());
        timenamer = file.ReadLine();
        timescaler = int.Parse(file.ReadLine());
        bufferint = int.Parse(file.ReadLine());
        for (int i = 0; i < bufferint; i++) {
            bufferstring = file.ReadLine();
            int bufint = int.Parse(file.ReadLine());
            float buffloat0 = float.Parse(file.ReadLine());
            float buffloat1 = float.Parse(file.ReadLine());
            float buffloat2 = float.Parse(file.ReadLine());
            importantEvents.Add(new ImportantEvent(bufferstring,bufint,new Color(buffloat0,buffloat1,buffloat2,1f)));
        }
        file.Close();
        importantEvents.Sort(delegate (ImportantEvent a, ImportantEvent b) {
            return a.position.CompareTo(b.position);
        });
       // importantEvents.Reverse();
        bufferindex = new List<int>();
        selectedcardindex = new List<int>();
        selectedcardindex.Add(0);
        if (!Directory.Exists(projectpath + "/maps")) { Directory.CreateDirectory(projectpath + "/maps"); }
        if (!Directory.Exists(projectpath + "/pictures")) { Directory.CreateDirectory(projectpath + "/pictures"); }
        maps = new List<string>(Directory.GetFiles(projectpath+"/maps"));
        List<string> buffermapslist = new List<string>();
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i][maps[i].Length - 4].ToString() + maps[i][maps[i].Length - 3].ToString() + maps[i][maps[i].Length - 2].ToString() + maps[i][maps[i].Length - 1].ToString() == ".png") {
                buffermapslist.Add(maps[i]);
            }
        }
        maps = buffermapslist;
        for (int i = 0; i < maps.Count; i++)
        {
            bufferstring = "";
            for (int ii = (projectpath + "/maps/").Length; ii < maps[i].Length; ii++) {
                bufferstring += maps[i][ii];
            }
            maps[i] = bufferstring;
        }
        for (int i = 0; i < maps.Count; i++) {
            bufferstring = "";
            for (int ii = 0; ii < maps[i].Length - 4; ii++) {
                bufferstring += maps[i][ii];
            }
                try{
                StreamReader file0 = new StreamReader(projectpath + "/maps/" + bufferstring + ".secomd");
                string bufstr = file0.ReadLine();
                bufferint = int.Parse(file0.ReadLine());
                int subint;
                Vector2Int bufvec = new Vector2Int();
                mapszones.Add(new MapZonesList(bufstr));
                for (int ii = 0; ii < bufferint; ii++) {
                    bufstr = file0.ReadLine();
                    buffercolor = new Color();
                    buffercolor.r = float.Parse(file0.ReadLine());
                    buffercolor.g = float.Parse(file0.ReadLine());
                    buffercolor.b = float.Parse(file0.ReadLine());
                    buffercolor.a = 0.58f;
                    subint = int.Parse(file0.ReadLine());
                    mapszones[i].mapZones.Add(new MapZone(bufstr,buffercolor));
                    for (int iii = 0; iii < subint; iii++) {
                        bufvec.x = int.Parse(file0.ReadLine());
                        bufvec.y = int.Parse(file0.ReadLine());
                        mapszones[i].mapZones[ii].points.Add(bufvec);
                    }
                    mapszones[i].mapZones[ii].Compose();
                }
                file0.Close();
            }
            catch {
                mapszones.Add(new MapZonesList(bufferstring));
            }
        }
        bufferstring = "";
        LoadTexts();
        issave = true;
        savetimer = 3000;
    }
    private void LoadPictures() {
        if (!Directory.Exists(projectpath+"/pictures")) {Directory.CreateDirectory(projectpath+ "/pictures"); }
        projectlist = new List<string>(Directory.GetFiles(projectpath+ "/pictures"));
        List<string> bufferprojectlist = new List<string>();
        for (int i = 0; i < projectlist.Count; i++) {
            if (projectlist[i][projectlist[i].Length - 4].ToString() + projectlist[i][projectlist[i].Length - 3].ToString() + projectlist[i][projectlist[i].Length - 2].ToString() + projectlist[i][projectlist[i].Length - 1].ToString()== ".png") {
                bufferprojectlist.Add(projectlist[i]);
            }
        }
        projectlist = bufferprojectlist;
        for (int i = 0; i < projectlist.Count; i++)
        {
            bufferstring = "";
            for (int ii = (projectpath + "/pictures/").Length; ii < projectlist[i].Length; ii++)
            {
                bufferstring += projectlist[i][ii];
            }
            projectlist[i]=bufferstring;
        }
        bufferstring = "";
    }
    private void LoadTexts() {
        if (!Directory.Exists(projectpath + "/texts")) { Directory.CreateDirectory(projectpath + "/texts"); }
        textslist = new List<string>(Directory.GetFiles(projectpath + "/texts"));
        List<string> buffertextslist = new List<string>();
        for (int i = 0; i < textslist.Count; i++)
        {
            if (textslist[i][textslist[i].Length - 4].ToString() + textslist[i][textslist[i].Length - 3].ToString() + textslist[i][textslist[i].Length - 2].ToString() + textslist[i][textslist[i].Length - 1].ToString() == ".txt")
            {
                buffertextslist.Add(textslist[i]);
            }
        }
        textslist = buffertextslist;
        for (int i = 0; i < textslist.Count; i++)
        {
            bufferstring = "";
            for (int ii = (projectpath + "/texts/").Length; ii < textslist[i].Length; ii++)
            {
                bufferstring += textslist[i][ii];
            }
            textslist[i] = bufferstring;
        }
        bufferstring = "";
    }
    private void LoadText(int selector) {
        StreamReader file = new StreamReader(projectpath + "/texts/" + textslist[selector]);
        //filetext = projectpath + "/texts/" + textslist[0] + file.ReadToEnd();
        stringlist = new List<string>();
        for (int i = 0; i < 10000; i++) {
            stringlist.Add(file.ReadLine());
            if(i>10)if(stringlist[i-1]+ stringlist[i - 2]+stringlist[i - 3]+stringlist[i - 4]+stringlist[i - 5]+stringlist[i - 6]+stringlist[i - 7]+stringlist[i - 8]+stringlist[i - 9]+stringlist[i - 10] == ""){ break; }
        }
        file.Close();
    }
    private void SaveText(int selector) {
        StreamWriter file = new StreamWriter(projectpath + "/texts/" + textslist[selector]);
        for (int i = 0; i < stringlist.Count; i++) {
            file.WriteLine(stringlist[i]);
        }
        file.Close();
    }
    private void LoadPresets() {
        if (!Directory.Exists(Application.dataPath + "/presets")) { Directory.CreateDirectory(Application.dataPath + "/presets"); }
        presetlist = new List<string>(Directory.GetFiles(Application.dataPath + "/presets"));
        List<string> bufferpresetlist = new List<string>();
        for (int i = 0; i < presetlist.Count; i++)
        {
            if (presetlist[i][presetlist[i].Length - 6].ToString() + presetlist[i][presetlist[i].Length - 5].ToString() + presetlist[i][presetlist[i].Length - 4].ToString() + presetlist[i][presetlist[i].Length - 3].ToString() + presetlist[i][presetlist[i].Length - 2].ToString() + presetlist[i][presetlist[i].Length - 1].ToString() == ".secop")
            {
                bufferpresetlist.Add(presetlist[i]);
            }
        }
        presetlist = bufferpresetlist;
        for (int i = 0; i < presetlist.Count; i++)
        {
            bufferstring = "";
            for (int ii = (Application.dataPath + "/presets/").Length; ii < presetlist[i].Length; ii++)
            {
                bufferstring += presetlist[i][ii];
            }
            presetlist[i] = bufferstring;
        }
        bufferstring = "";
    }
    private List<Block> LoadPreset(string path) {
        List<Block> blocks = new List<Block>();
        StreamReader file = new StreamReader(path);
        int bufint = int.Parse(file.ReadLine());
        for (int i = 0; i < bufint; i++) {
            bool image = bool.Parse(file.ReadLine());
            bool note = bool.Parse(file.ReadLine());
            Rect rect = new Rect();
            rect.x = float.Parse(file.ReadLine());
            rect.y = float.Parse(file.ReadLine());
            rect.width = float.Parse(file.ReadLine());
            rect.height = float.Parse(file.ReadLine());
            string buffername = file.ReadLine();
            int bufferint = int.Parse(file.ReadLine());
            bool bool0 = bool.Parse(file.ReadLine());
            bool bool1 = bool.Parse(file.ReadLine());
            Block block = new Block(rect, buffername);
            block.image = image;
            block.note = note;
            block.time = bufferint;
            block.timegroup = bool0;
            block.timezonetype = bool1;
            blocks.Add(block);
        }
        file.Close();
        return blocks;
    }
    private void ApplyPreset(int presetid) {
        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks = LoadPreset(Application.dataPath + "/presets/"+presetlist[presetid]);
    }
    private void AddPreset(Card card, string Name) {
        StreamWriter file = new StreamWriter(Application.dataPath + "/presets/"+Name+".secop");
        file.WriteLine(card.blocks.Count);
        for (int i = 0; i < card.blocks.Count; i++) {
            file.WriteLine(card.blocks[i].image);
            file.WriteLine(card.blocks[i].note);
            file.WriteLine(card.blocks[i].rect.x);
            file.WriteLine(card.blocks[i].rect.y);
            file.WriteLine(card.blocks[i].rect.width);
            file.WriteLine(card.blocks[i].rect.height);
            file.WriteLine(card.blocks[i].text);
            file.WriteLine(card.blocks[i].time);
            file.WriteLine(card.blocks[i].timegroup);
            file.WriteLine(card.blocks[i].timezonetype);
        }
        file.Close();
        presetlist.Add("Name");
    }
    private void DestroyPreset(int presetid) {
        File.Delete(Application.dataPath + "/presets/"+presetlist[presetid]);
        presetlist.Remove(presetlist[presetid]);
    }
    private void Start()
    {
        if (!Directory.Exists(Application.dataPath + "/projects")) {Directory.CreateDirectory(Application.dataPath + "/projects"); }
        if (!Directory.Exists(Application.dataPath + "/presets")) {Directory.CreateDirectory(Application.dataPath + "/presets"); }
        projectlist = new List<string>(Directory.GetDirectories(Application.dataPath + "/projects"));
        datelist = new List<string>();
        for (int i = 0; i < projectlist.Count; i++)
        {
            bufferstring = "";
            for (int ii = (Application.dataPath + "/projects/").Length; ii < projectlist[i].Length; ii++)
            {
                bufferstring += projectlist[i][ii];
            }
            projectlist[i] = bufferstring;
            datelist.Add(Directory.GetCreationTime(projectlist[i]).ToUniversalTime().ToString());
        }
        bufferstring = "";
        importantEvents = new List<ImportantEvent>();
        graytexture = new Texture2D(1, 1);
        graytexture.SetPixel(0, 0, new Color(.5568f, .5568f, .5568f, 1f));
        graytexture.Apply();
        greentexture = new Texture2D(1, 1);
        greentexture.SetPixel(0, 0, new Color(.3568f, .5568f, .3568f, 1f));
        greentexture.Apply();
        litegreentexture = new Texture2D(1, 1);
        litegreentexture.SetPixel(0, 0, new Color(.2568f, .6568f, .2568f, 1f));
        litegreentexture.Apply();
        buffercolortexture = new Texture2D(1, 1);
        buffercolortexture.SetPixel(0,0, new Color(0, 0, 0, 1f));
        buffercolortexture.Apply();
        buffercolor = new Color(0,0,0,1f);
        orangetexture = new Texture2D(1, 1);
        orangetexture.SetPixel(0, 0, new Color(.7f, .35f, .0f, .125f));
        orangetexture.Apply();
        mapszones = new List<MapZonesList>();
        //   InitializeProject();
        maps = new List<string>();
        textslist = new List<string>();
    }
    public class MapZonesList {
        public string name { get; set; }
        public List<MapZone> mapZones { get; set; }
        public MapZonesList(string Name) {
            name = Name;
            mapZones = new List<MapZone>();
        }
    }
    public class MapZone{
        public List<Vector2Int> points { get; set; }
        public Texture2D zonepicture { get; set; }
        public string name { get; set; }
        public byte[,] zonebinarymap { get; set; }
        public Color color { get; set; }
        public Texture2D colortexture { get; set; }
        public bool show { get; set; }
        public MapZone(string Name) {
            name = Name;
            zonepicture = new Texture2D(Screen.width - 400, Screen.height - 200);
            zonebinarymap = new byte[Screen.width-400,Screen.height-200];
            for (int ii = 0; ii < zonebinarymap.GetLength(1); ii++) for (int i = 0; i < zonebinarymap.GetLength(0); i++) {
                    zonebinarymap[i, ii] = 0;
                    zonepicture.SetPixel(i, ii, new Color(0, 0, 0, 0));
                }
            zonepicture.Apply();
            points = new List<Vector2Int>();
            color = new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.58f);
            colortexture = new Texture2D(1, 1);
            colortexture.SetPixel(0, 0, color);
            colortexture.Apply();
            show = true;
        }
        public MapZone(string Name, Color Color)
        {
            name = Name;
            zonepicture = new Texture2D(Screen.width - 400, Screen.height - 200);
            zonebinarymap = new byte[Screen.width - 400, Screen.height - 200];
            for (int ii = 0; ii < zonebinarymap.GetLength(1); ii++) for (int i = 0; i < zonebinarymap.GetLength(0); i++)
                {
                    zonebinarymap[i, ii] = 0;
                    zonepicture.SetPixel(i, ii, new Color(0, 0, 0, 0));
                }
            zonepicture.Apply();
            points = new List<Vector2Int>();
            color = Color;
            colortexture = new Texture2D(1, 1);
            colortexture.SetPixel(0, 0, color);
            colortexture.Apply();
        }
        public void Compose() {
            float plusx, plusy;
            Vector2Int pos;
            Vector2 bufpos;
            Vector2Int middlepoint=new Vector2Int();
            Vector2Int leftup = new Vector2Int(), rightdown = new Vector2Int();
            Color32[] colors32 = new Color32[zonepicture.width * zonepicture.height];
            for (int i = 0; i < points.Count; i++) {
                middlepoint.x += points[i].x;
                middlepoint.y += points[i].y;
                if (points[i].x > rightdown.x) { rightdown.x = points[i].x; }
                if (points[i].y < rightdown.y) { rightdown.y = points[i].y; }
                if (points[i].x < leftup.x) { leftup.x = points[i].x; }
                if (points[i].y > leftup.y) { leftup.y = points[i].y; }
                if (i < points.Count - 1)
                {
                    pos = points[i];
                    bufpos = pos;
                    plusx = (points[i].x - points[i+1].x) / 400f;
                    plusy = (points[i].y - points[i+1].y) / 400f;
                    for (int hi = 0; hi < 400; hi++)
                    {
                        bufpos.x -= plusx;
                        bufpos.y -= plusy;
                        pos = new Vector2Int((int)bufpos.x, (int)bufpos.y);
                        zonebinarymap[pos.x, pos.y] = 1;
                    }
                }
                else
                {
                    pos = points[i];
                    bufpos = pos;
                    plusx = (points[i].x - points[0].x) / 400f;
                    plusy = (points[i].y - points[0].y) / 400f;
                    for (int hi = 0; hi < 400; hi++)
                    {
                        bufpos.x -= plusx;
                        bufpos.y -= plusy;
                        pos = new Vector2Int((int)bufpos.x, (int)bufpos.y);
                        zonebinarymap[pos.x, pos.y] = 1;
                    }

                }
            }
            middlepoint.x /= points.Count;
            middlepoint.y /= points.Count;
            zonebinarymap[middlepoint.x, middlepoint.y]=2;
            bool edited=false;
            for (int i = 0; i < 512; i++)
            {
                edited = true;
                for (int iy = rightdown.y; iy < leftup.y; iy++) for (int ix = leftup.x; ix < rightdown.x; ix++)if(zonebinarymap[ix,iy]==2)
                    {
                            if (iy > 0) if (zonebinarymap[ix, iy - 1] == 0) { zonebinarymap[ix, iy - 1] = 2; edited = false; }
                            if (iy < zonebinarymap.GetLength(1)-1) if (zonebinarymap[ix, iy + 1] == 0) { zonebinarymap[ix, iy + 1] = 2; edited = false; }
                            if (ix > 0) if (zonebinarymap[ix-1, iy] == 0) { zonebinarymap[ix-1, iy] = 2; edited = false; }
                            if (ix < zonebinarymap.GetLength(0)-1) if (zonebinarymap[ix + 1, iy] == 0) { zonebinarymap[ix + 1, iy] = 2; edited = false; }
                    }
                if (edited) { break;}
            }
            int pixels=0;
            int zpwidth = zonepicture.width;
            for (int iy = rightdown.y; iy < leftup.y; iy++) for (int ix = leftup.x; ix < rightdown.x; ix++) if (zonebinarymap[ix, iy] == 2) {
                        //zonepicture.SetPixel(ix,zonepicture.height-iy,color);
                        colors32[ix + (zonepicture.height - iy) * zpwidth] = color;
                        pixels++;
            }
            zonepicture.SetPixels32(colors32);
            zonepicture.Apply();
        }
    }
    public class ImportantEvent {
        public string name { get; set; }
        public int position { get; set; }
        public Color color { get; set; }
        public Texture2D texture { get; set; }
        public ImportantEvent(string Name, int Position, Color Color) {
            name = Name;
            position = Position;
            color = Color;
            texture = new Texture2D(1,1);
            texture.SetPixel(0,0,color);
            texture.Apply();
        }
    }
    public class Block
    {
        public Rect rect { get; set; }
        public Texture2D picture { get; set; }
        public string text { get; set; }
        public bool image { get; set; }
        public bool note { get; set; }
        public int time { get; set; }
        public bool timegroup { get; set; }
        public bool timezonetype { get; set; }
        public Block(Rect Rect, Texture2D Picture,string Text)
        {
            rect = Rect;
            text = Text;
            picture = Picture;
            image = true;
            note = false;
            time = -1;
        }
        public Block(Rect Rect, string Text)
        {
            rect = Rect;
            text = Text;
            image = false;
            note = false;
            time = -1;
        }
        public Block(Rect Rect, string Text, bool Note)
        {
            rect = Rect;
            text = Text;
            image = false;
            note = true;
            time = -1;
        }
    }
    public class Card
    {
        public List<int> index { get; set; }
        public string name { get; set; }
        public List<Card> childrens { get; set; }
        public List<Block> blocks { get; set; }
        public Card(string Name, List<int> Index)
        {
            name = Name;
            index = new List<int>();
            for (int i = 0; i < Index.Count; i++) { index.Add(Index[i]); }
            childrens = new List<Card>();
            blocks = new List<Block>();
        }
    }
    private void show(Card card, int recursiondeep)
    {
        if (GUI.Button(new Rect(Screen.width - 200 + recursiondeep * 5, (megacounter-scroller) * 25, 200 - recursiondeep * 5, 25), "" + card.name, button))
        {
            if (!movenow)
            {
                selectedcardindex = card.index;
            }
            else
            {
                movenow = false;
                GetCard(cards[card.index[0]], card.index, 1).childrens.Add(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1));
                for (int ii = 0; ii < selectedcardindex.Count - 1; ii++)
                {
                    bufferindex.Add(selectedcardindex[ii]);
                }
                GetCard(cards[selectedcardindex[0]], bufferindex, 1).childrens.Remove(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1));
                selectedcardindex = bufferindex;
            }
        }
        recursiondeep++;
        megacounter++;
        if (card.childrens.Count > 0) for (int i = 0; i < card.childrens.Count; i++)
            {
                //    megacounter++;
                show(card.childrens[i], recursiondeep);
            }
    }
    private void ShowWithTimeLine(int time, Card card,int deep) {
        if (deep > 100) { return; }
        for (int i = 0; i < card.blocks.Count; i++) {
            if (card.blocks[i].time == time&&card.blocks[i].timegroup) {
                GUI.Box(new Rect(1*time,Screen.height-200+deep,13*(card.name.Length+2+card.blocks[i].text.Length),25),card.name+": "+card.blocks[i].text,boxtexture);
                GUI.DrawTexture(new Rect(1 * time, Screen.height - 200 + deep, 1, 100 - deep),graytexture);
                deep+=25;
            }
        }
        for (int i = 0; i < card.childrens.Count; i++) {
            ShowWithTimeLine(time,card.childrens[i],deep);
        }
    }
    private void ShowWithNonStaticTimeLine(int time,Card card,int deep) {
        if (deep > 100) { return; }
        for (int i = 0; i < card.blocks.Count; i++) 
        {
            if (card.blocks[i].time == time && !card.blocks[i].timegroup)
            {
                if (card.blocks[i].timezonetype)
                {
                    GUI.Box(new Rect(importantEvents[time].position, Screen.height - 75 + deep, 13 * (card.name.Length + 2 + card.blocks[i].text.Length), 25), card.name + ": " + card.blocks[i].text, boxtexture);
                    GUI.DrawTexture(new Rect(importantEvents[time].position, Screen.height - 75 - deep, 1, 100 - deep), graytexture);
                }
                else
                {
                    GUI.Box(new Rect(0, Screen.height - 100 - deep, 13 * (card.name.Length + 2 + card.blocks[i].text.Length), 25), card.name + ": " + card.blocks[i].text, boxtexture);
                    GUI.DrawTexture(new Rect(0, Screen.height - 100 - deep, 1, 100 - deep), graytexture);
                }
                deep+=25;
            }
        }
        for (int i = 0; i < card.childrens.Count; i++)
        {
            ShowWithNonStaticTimeLine(time, card.childrens[i], deep);
        }
    }
    private Card GetCard(Card card, List<int> index, int step)
    {
        if (index.Count == step)
        {
            return card;
        }
        else
        {
            card = GetCard(card.childrens[index[step]], index, step + 1);
        }
        return card;
    }
    private void Update()
    {
        if (guiphase == 1)
        {
            if (Input.mousePosition.x < Screen.width - 200 && Input.mousePosition.x > 200 && Input.mousePosition.y > 200 && scaletype == 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1)&&!Input.GetKey(KeyCode.Space))
                {
                    rightselector = true;
                    point = new Vector2Int((int)Input.mousePosition.x, Screen.height - (int)Input.mousePosition.y);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (rightselector) if (Input.mousePosition.x < point.x || Input.mousePosition.x > point.x + 250 || Screen.height - Input.mousePosition.y < point.y || Screen.height - Input.mousePosition.y > point.y + 75)
                        {
                            rightselector = false;
                        }
                }
            }
            if (Input.mousePosition.y < 200 && Input.mousePosition.y > 100)
                if (!addimportantevent)
                {
                    if (!creategreatpoint)
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            timeselector = ((int)(Input.mousePosition.x / 2)) * 2;
                            timelinegroup = true;
                        }
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            greatpointpositionselector = ((int)(Input.mousePosition.x / 2)) * 2;
                            timelinegroup = true;
                        }
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        buffertimepos = ((int)(Input.mousePosition.x / 2)) * 2;
                    }
                }
            else if (Input.mousePosition.y < 100)
            {
                if (!creategreatpoint && !addimportantevent) if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (Input.mousePosition.x < importantEvents[0].position)
                        {
                            timezonetype = false;
                            timezoneselector = 0;
                        }
                        if (Input.mousePosition.x > importantEvents[importantEvents.Count - 1].position)
                        {
                            timezoneselector = importantEvents.Count - 1;
                            timezonetype = true;
                        }
                        for (int i = 0; i < importantEvents.Count - 1; i++)
                        {
                            if (Input.mousePosition.x > importantEvents[i].position && Input.mousePosition.x < importantEvents[i + 1].position)
                            {
                                timezoneselector = i;
                                timezonetype = true;
                            }
                        }
                        timelinegroup = false;
                    }
            }
            if (drag == true)
            {
                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect = new Rect(Input.mousePosition.x - GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.width, Screen.height - Input.mousePosition.y, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.width, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.height);
            }


            if (scaletype == 1)
            {
                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect = new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.x, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.y, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.width * (1f + Input.GetAxis("Mouse X") * 0.1f), GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.height);
                if (Input.GetKeyDown(KeyCode.Mouse1)) { scaletype = 0; }
            }
            if (scaletype == 2)
            {
                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect = new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.x, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.y, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.width, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.height * (1f + Input.GetAxis("Mouse Y") * -0.1f));
                if (Input.GetKeyDown(KeyCode.Mouse1)) { scaletype = 0; }
            }
            if (scaletype == 3)
            {
                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect = new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.x, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.y, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.width * (1f + Input.GetAxis("Mouse X") * 0.1f), GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[currentdrag].rect.height * (1f + Input.GetAxis("Mouse Y") * -0.1f));
                if (Input.GetKeyDown(KeyCode.Mouse1)) { scaletype = 0; }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.S)) { SaveProject(); print("Save"); }
            if (guisection == 2) if (textselector != -1)
                {
                    if (Input.GetAxis("Mouse ScrollWheel") < 0f) { linecounter++; }
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f) { if (linecounter > 0) { linecounter--; } }
                    if (linecounter > stringlist.Count + (Screen.height / 50) + 1) { linecounter = stringlist.Count + (Screen.height / 50) + 1; }
                }
        if (guisection == 0) {
            if (zoneeditmode)
            {
                if (Input.mousePosition.x > 200 && Input.mousePosition.x < Screen.width - 200 && Input.mousePosition.y > 200&&!((Input.mousePosition.x>Screen.width-250&&Input.mousePosition.y<225)|| (Input.mousePosition.x > Screen.width - 225 && Input.mousePosition.y < 250)))
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                            if (!pointeditmode)
                            {
                                mapszones[mapselector].mapZones[selectedzone].points.Add(new Vector2Int((int)((Input.mousePosition.x - 200 - shift.x) / scale), (int)((Screen.height - Input.mousePosition.y- shift.y)/ scale )));
                            }
                            else
                            {
                                List<Vector2Int> bufint = new List<Vector2Int>();
                                bufint.Add(new Vector2Int((int)((Input.mousePosition.x - 200 - shift.x) / scale), (int)((Screen.height - Input.mousePosition.y- shift.y) / scale )));
                                mapszones[mapselector].mapZones[selectedzone].points = bufint;
                            }
                    }
                }
            }
                if (Input.GetKey(KeyCode.Mouse2)||(Input.GetKey(KeyCode.Mouse1)&&Input.GetKey(KeyCode.Space)))
                {
                    shift += new Vector2(Input.GetAxis("Mouse X") * 9f*(Screen.width/selectedmap.width), Input.GetAxis("Mouse Y") * -9f * ((Screen.height*1f) / (selectedmap.height*1f))); 
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f||Input.GetKeyDown(KeyCode.Equals)) { scale += 0.1f;}
                if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.Minus)) { scale -= 0.1f; }
            }
                if ((guisection == 3||guisection==0 || guisection == 4)&&Input.mousePosition.x>Screen.width-200) {
                    if (Input.GetAxis("Mouse ScrollWheel") < 0f) { if (scroller < megacounter - 1) { scroller++; } }
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f) { if (scroller > 0) { scroller--; } }
                }
            if (Input.GetKey(KeyCode.LeftControl)&&Input.GetKey(KeyCode.Alpha0))
            {
                scale = (1f * Screen.height - 200) / (1f * selectedmap.height);
                shift = new Vector2((Screen.width - 400) * 0.5f - (selectedmap.width * scale * 0.5f), 0);
            }
        }
        if (issave) { if (savetimer > 0) { --savetimer; } else { issave = false; } }
    }
    private void OnGUI()
    {
        if (guiphase == 0)
        {
            GUI.Box(new Rect(Screen.width*0.5f-150, 0, 300, 60), "Существующие проекты:", boxtexture);
            for (int i = 0; i < projectlist.Count; i++)
            {
                if (GUI.Button(new Rect(Screen.width*0.5f-150, 60 + i * 60, 300, 60), projectlist[i], button))
                {
                    projectpath = Application.dataPath + "/projects/"+projectlist[i];
                    projectname = projectlist[i];
                    LoadProject();
                    LoadPictures();
                    guiphase = 1;
                }
                //GUI.Box(new Rect(500, 60 + i * 60, 200, 60), datelist[i], boxtexture);
            }
            if (GUI.Button(new Rect(Screen.width*0.5f+150, 0, 300, 60), "Создать новый проект", button)) { InitializeProject(); guiphase = 2; }
            if (GUI.Button(new Rect(0, Screen.height - 20, 120, 20), "Выйти", button)) { Application.Quit(); }
        }
        else if (guiphase == 1)
        {
            //корректное отоброжение карты
            if (guisection == 0 && maps.Count > 0)
            {
                if (mapselector != -1)
                {
                    GUI.DrawTexture(new Rect(200 + shift.x, 0 + shift.y, selectedmap.width * scale, selectedmap.height * scale), selectedmap);
                }
                if(zonemode)for (int i = 0; i < mapszones[mapselector].mapZones.Count; i++)
                {
                    if (!mapszones[mapselector].mapZones[i].show) GUI.DrawTexture(new Rect(200 + shift.x, 0 + shift.y, mapszones[mapselector].mapZones[i].zonepicture.width * scale, mapszones[mapselector].mapZones[i].zonepicture.height * scale), mapszones[mapselector].mapZones[i].zonepicture);

                if (mapszones[mapselector].mapZones[i].points.Count != 1)
                {
                    if (zoneeditmode && selectedzone == i) for (int ii = 0; ii < mapszones[mapselector].mapZones[i].points.Count; ii++)
                        {
                            GUI.Box(new Rect((mapszones[mapselector].mapZones[i].points[ii].x) * scale + shift.x + 198, (mapszones[mapselector].mapZones[i].points[ii].y) * scale + shift.y - 2, 5, 5), "", boxtexture);
                        }
                }
                else
                {
                    GUI.Box(new Rect((mapszones[mapselector].mapZones[i].points[0].x) * scale + shift.x + 198, (mapszones[mapselector].mapZones[i].points[0].y) * scale + shift.y - 2, 5, 5), "", boxtexture);
                    GUI.DrawTexture(new Rect((mapszones[mapselector].mapZones[i].points[0].x) * scale + shift.x + 198, (mapszones[mapselector].mapZones[i].points[0].y) * scale + shift.y - 2, 5, 5), mapszones[mapselector].mapZones[i].colortexture);
                    GUI.Box(new Rect((mapszones[mapselector].mapZones[i].points[0].x) * scale + shift.x + 200 - (mapszones[mapselector].mapZones[i].name.Length * 13) / 2, (mapszones[mapselector].mapZones[i].points[0].y) * scale + shift.y - 30, mapszones[mapselector].mapZones[i].name.Length * 13, 25), mapszones[mapselector].mapZones[i].name, boxtexture);
                }
                }
            }
            //основная разметка
            GUI.Box(new Rect(0, -1, 200, Screen.height - 198), "", boxtexture);
            GUI.Box(new Rect(Screen.width - 200, -1, 200, Screen.height - 198), "", boxtexture);
            GUI.Box(new Rect(0, Screen.height - 200, Screen.width, 200), "", boxtexture);
            if (leftmenu)
            {
                if (GUI.Button(new Rect(0, 60, 200, 25), "Сохранить", button)) { SaveProject(); leftmenu = false; }
                if (GUI.Button(new Rect(0, 85, 200, 25), "Меню", button)) { SaveProject(); Application.LoadLevel(0); }
                if (GUI.Button(new Rect(0, 110, 200, 25), "О программе", button)) { showinfo = true; leftmenu = false; }
                if (GUI.Button(new Rect(0, 135, 200, 25), "Помощь", button)) { showhelp = true; leftmenu = false; }
                if (GUI.Button(new Rect(0, 160, 200, 25), "Выход", button)) { SaveProject(); Application.Quit(); }
            }
            if (issave) {
                GUI.Box(new Rect(200, 0, 20, 20), savepic,boxtexture);
            }
            //таймлайн
            GUI.Box(new Rect(0,Screen.height-200,105,25),"Таймлайн",boxtexture);
            GUI.DrawTexture(new Rect(0,Screen.height-100,Screen.width,1),graytexture);
            if(timelinegroup)GUI.DrawTexture(new Rect(timeselector,Screen.height-200,1,100),graytexture);
            GUI.DrawTexture(new Rect(greatpointpositionselector, Screen.height - 200, 1, 200), litegreentexture);
            for (int i = 0; i < Screen.width; i++) {
                GUI.DrawTexture(new Rect(i * 2, Screen.height - 100 - 1* Mathf.FloorToInt(Mathf.Floor(i * 0.5f) / Mathf.Ceil(i * 0.5f)) - 2 * Mathf.FloorToInt(Mathf.Floor(i * 0.1f) / Mathf.Ceil(i * 0.1f)) - 3 * Mathf.FloorToInt(Mathf.Floor(i * 0.01f) / Mathf.Ceil(i * 0.01f)) - 4 * Mathf.FloorToInt(Mathf.Floor(i * 0.001f) / Mathf.Ceil(i * 0.001f)), 1, 1 * Mathf.FloorToInt(Mathf.Floor(i * 0.5f) / Mathf.Ceil(i * 0.5f)) + 2 * Mathf.FloorToInt(Mathf.Floor(i * 0.1f) / Mathf.Ceil(i * 0.1f)) + 3 * Mathf.FloorToInt(Mathf.Floor(i * 0.01f) / Mathf.Ceil(i * 0.01f)) + 4 * Mathf.FloorToInt(Mathf.Floor(i * 0.001f) / Mathf.Ceil(i * 0.001f))),graytexture);
                for (int c = 0; c < cards.Count; c++) {
                    ShowWithTimeLine(i,cards[c],0);
                }
            }
            for (int i = 0; i < importantEvents.Count; i++) {
                for (int c = 0; c < cards.Count; c++)
                {
                    ShowWithNonStaticTimeLine(i, cards[c], 0);
                }
            }
            if (!timelinegroup)
            {
                if (timezonetype == false && timezoneselector == 0)
                {
                    GUI.DrawTexture(new Rect(0, Screen.height - 100, importantEvents[0].position, 100), orangetexture);
                }
                else if (timezonetype == true && timezoneselector == importantEvents.Count - 1)
                {
                    GUI.DrawTexture(new Rect(importantEvents[importantEvents.Count - 1].position, Screen.height - 100, Screen.width - importantEvents[importantEvents.Count - 1].position, 100), orangetexture);
                }
            }
            for (int i = 0; i < importantEvents.Count; i++) {
                GUI.Box(new Rect(importantEvents[i].position,Screen.height-25,importantEvents[i].name.Length*13,25),importantEvents[i].name,boxtexture);
                GUI.DrawTexture(new Rect(importantEvents[i].position,Screen.height-125,1,125),importantEvents[i].texture);
                if(!timelinegroup)if (i < importantEvents.Count - 1) {
                    if (timezonetype&&timezoneselector == i)
                    {
                        GUI.DrawTexture(new Rect(importantEvents[i].position, Screen.height - 100, importantEvents[i+1].position - importantEvents[i].position, 100), orangetexture);
                    }
                }
            }
            if (timeselector != -1&&timelinegroup) {
                if (greatpointname == "")
                {
                    GUI.Box(new Rect(Screen.width - (timenamer + " " + timescaler * timeselector).Length * 13, Screen.height - 200, (timenamer + " " + timescaler * timeselector).Length * 13, 25), timenamer + " " + timescaler * timeselector, boxtexture);
                }
                else {
                    if (timeselector < greatpointpositionselector)
                    {
                        GUI.Box(new Rect(Screen.width - (timenamer + " " + (timescaler * (greatpointpositionselector - timeselector)) + " до " + greatpointname).Length * 12, Screen.height - 200, (timenamer + " " + (timescaler * (greatpointpositionselector - timeselector)) + " до " + greatpointname).Length * 12, 25), timenamer + " " + (timescaler * (greatpointpositionselector - timeselector)) + " до " + greatpointname, boxtexture);
                    }
                    else {
                        GUI.Box(new Rect(Screen.width - (timenamer + " " + (timescaler * (timeselector - greatpointpositionselector)) + " после " + greatpointname).Length * 12, Screen.height - 200, (timenamer + " " + (timescaler * (timeselector - greatpointpositionselector)) + " после " + greatpointname).Length * 12, 25), timenamer + " " + (timescaler * (timeselector-greatpointpositionselector)) + " после " + greatpointname, boxtexture);
                    }
                }
            } else {
                GUI.Box(new Rect(Screen.width - (timenamer+": неизвестно").Length*13,Screen.height-200,(timenamer+": неизвестно").Length*13,25),timenamer+": неизвестно",boxtexture);
            }
            //глобальные разделы
            if (GUI.Button(new Rect(0, Screen.height * 0.3f - 25, 200, 25), "Карты", button)) { guisection = 0; }
            if (GUI.Button(new Rect(0, Screen.height * 0.3f, 200, 25), "История", button)) { guisection = 1; }
            if (GUI.Button(new Rect(0, Screen.height * 0.3f + 25, 200, 25), "Текст", button)) { guisection = 2; }
            if (GUI.Button(new Rect(0, Screen.height * 0.3f + 50, 200, 25), "Карточки", button)) { guisection = 3; }
            if (guisection != 4) { GUI.Box(new Rect(0, Screen.height * 0.3f - 25 + 25 * guisection, 200, 25), "", selectedbutton); }
            for (int i = 0; i < arbitraryselections.Count; i++)
            {
                if (GUI.Button(new Rect(0, Screen.height * 0.3f + 125 + 25 * i, 175, 25), "" + arbitraryselections[i], button)) {
                    guisection = 4; selectedselection = i;
                    selectedcardindex = cards[selectedselection].index;
                }
                if (GUI.Button(new Rect(175, Screen.height * 0.3f + 125 + 25 * i, 25, 25), trashpic, button)) {
                    arbitraryselections.Remove(arbitraryselections[i]);
                    cards.Remove(cards[i]);
                }
            }
            if (Screen.height * 0.3f + 150 + 25 * arbitraryselections.Count < Screen.height - 200) if (GUI.Button(new Rect(0, Screen.height * 0.3f + 125 + 25 * arbitraryselections.Count, 200, 25), "Добавить раздел", button)) { addsection = true; }
            if (guisection == 0&&maps.Count>0)
            {
                if (!zonemode)
                {
                    megacounter = 0;
                    for (int i = 0; i < maps.Count; i++)
                    {
                        megacounter++;
                        if (mapselector != i)
                        {
                            if (GUI.Button(new Rect(Screen.width - 200, 0 + 25 * (i-scroller), 200, 25), maps[i], button))
                            {
                                mapselector = i;
                                selectedmap = new Texture2D(2, 2);
                                selectedmap.LoadImage(File.ReadAllBytes(projectpath + "/maps/" + maps[i]));
                                scale =  (1f*Screen.height-200)/(1f * selectedmap.height);
                                shift = new Vector2((Screen.width-400)*0.5f-(selectedmap.width*scale*0.5f),0);
                            }
                        }
                        else
                        {
                            GUI.Box(new Rect(Screen.width - 200, 0 + 25 * (i - scroller), 200, 25), maps[i], boxtexture);
                        }
                    }
                }
                else {
                    megacounter = 0;
                    for (int i = 0; i < mapszones[mapselector].mapZones.Count; i++)
                    {
                        megacounter++;
                        if (GUI.Button(new Rect(Screen.width - 200, (i-scroller) * 25, 175, 25), mapszones[mapselector].mapZones[i].name, button))
                        {
                            selectedzone = i;
                        }
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (GUI.Button(new Rect(Screen.width - 25, (i - scroller) * 25, 25, 25), trashpic, button))
                            {
                                mapszones[mapselector].mapZones.Remove(mapszones[mapselector].mapZones[i]);
                            }
                        }
                        else
                        {
                            if (GUI.Button(new Rect(Screen.width - 25, (i - scroller) * 25, 25, 25), eyepic, button))
                            {
                                mapszones[mapselector].mapZones[i].show = !mapszones[mapselector].mapZones[i].show;
                            }
                        }
                        if (selectedzone == i)
                        {
                            GUI.Box(new Rect(Screen.width - 200, (i - scroller) * 25, 4, 25), "", boxtexture);
                        }
                        GUI.DrawTexture(new Rect(Screen.width - 200, (i - scroller) * 25, 5, 25), mapszones[mapselector].mapZones[i].colortexture);
                        
                    }
                    if (GUI.Button(new Rect(Screen.width - 200, (mapszones[mapselector].mapZones.Count-scroller) * 25, 200, 25), "Добавить зону", button)) {
                        zoneadd = true;
                        buffercolor = new Color(0,0,0,1f);
                        buffercolortexture.SetPixel(0, 0, buffercolor);
                        buffercolortexture.Apply();
                    }
                    if (zoneadd) {
                        GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,400),"",boxtexture);
                        GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,25),"Добавить зону",boxtexture);
                        GUI.Box(new Rect(Screen.width * 0.5f-200, Screen.height * 0.4f - 100, 200, 25), "Название: ", boxtexture);
                        bufferstring = GUI.TextField(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 100, 200, 25), bufferstring, boxtexture);
                        GUI.Box(new Rect(Screen.width * 0.5f-200, Screen.height * 0.4f - 75, 175, 25), "Цвет: ", boxtexture);
                        if (GUI.Button(new Rect(Screen.width * 0.5f - 25, Screen.height * 0.4f - 75, 25, 25), dicepic, button))
                        {
                            if (Random.Range(0, 2) == 0)
                            {
                                buffercolor.r = Random.Range(0f, 1f);
                                buffercolor.g = buffercolor.r / 2;
                                buffercolor.b = buffercolor.r / 2;
                            }
                            else if (Random.Range(0, 2) == 0)
                            {
                                buffercolor.g = Random.Range(0f, 1f);
                                buffercolor.b = buffercolor.r / 2;
                                buffercolor.r = buffercolor.r / 2;
                            }
                            else
                            {
                                buffercolor.b = Random.Range(0f, 1f);
                                buffercolor.r = buffercolor.r / 2;
                                buffercolor.g = buffercolor.r / 2;
                            }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 75, 200, 25), buffercolortexture);
                        GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 50, 200, 25), "R:" + (int)(buffercolor.r / 0.0039f), boxtexture);
                        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 50, 25, 25), "+", button))
                        {
                            if (buffercolor.r < 1f) { buffercolor.r += 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 25, Screen.height * 0.4f - 50, 50, 25), dicepic, button))
                        {
                            buffercolor.r = Random.Range(0f, 1f);
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 75, Screen.height * 0.4f - 50, 25, 25), "-", button))
                        {
                            if (buffercolor.r > 0.0039f) { buffercolor.r -= 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 200, 25), "G:" + (int)(buffercolor.g / 0.0039f), boxtexture);
                        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 25, 25, 25), "+", button))
                        {
                            if (buffercolor.g < 1f) { buffercolor.g += 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 25, Screen.height * 0.4f - 25, 50, 25), dicepic, button))
                        {
                            buffercolor.g = Random.Range(0f, 1f);
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 75, Screen.height * 0.4f - 25, 25, 25), "-", button))
                        {
                            if (buffercolor.g > 0.0039f) { buffercolor.g -= 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f, 200, 25), "B:" + (int)(buffercolor.b / 0.0039f), boxtexture);
                        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f, 25, 25), "+", button))
                        {
                            if (buffercolor.b < 1f) { buffercolor.b += 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 25, Screen.height * 0.4f, 50, 25), dicepic, button))
                        {
                            buffercolor.b = Random.Range(0f, 1f);
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f + 75, Screen.height * 0.4f, 25, 25), "-", button))
                        {
                            if (buffercolor.b > 0.0039f) { buffercolor.b -= 0.0039f; }
                            buffercolortexture.SetPixel(0, 0, buffercolor);
                            buffercolortexture.Apply();
                        }
                        if (GUI.Button(new Rect(Screen.width*0.5f-200,Screen.height*0.4f+175,200,25),"Отмена",button)) {
                            bufferstring = "";
                            zoneadd = false;
                        }
                        if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f + 175, 200, 25), "Добавить зону", button))
                        {
                            buffercolor.a = 0.58f;
                            mapszones[mapselector].mapZones.Add(new MapZone(bufferstring, buffercolor));
                            bufferstring = "";
                            zoneadd = false;
                        }
                    }
                    if(selectedzone!=-1)if (GUI.Button(new Rect(Screen.width - 250, Screen.height - 225, 25, 25), "E", button)) {
                        if (zoneeditmode) {
                            zoneeditmode = false;
                            mapszones[mapselector].mapZones[selectedzone].Compose();
                        }
                        else {
                            zoneeditmode = true;
                        }
                    }
                    if (zoneeditmode) {
                        GUI.Box(new Rect(Screen.width - 250, Screen.height - 225, 25, 25), "", selectedbutton);
                    }
                    if (GUI.Button(new Rect(Screen.width - 225, Screen.height - 250, 25, 25), "P", button)) {
                        if (pointeditmode)
                        {
                            pointeditmode = false;
                        }
                        else {
                            pointeditmode = true;
                        }
                    }
                    if (pointeditmode) {
                        GUI.Box(new Rect(Screen.width - 225, Screen.height - 250, 25, 25), "", selectedbutton);
                    }
                }
                if(mapselector!=-1)if (GUI.Button(new Rect(Screen.width - 225, Screen.height - 225, 25, 25), "Z", button))
                {
                    if (zonemode)
                    {
                        zonemode = false;
                        scroller = 0;
                    }
                    else
                    {
                        zonemode = true;
                        scroller = 0;
                        selectedzone = -1;
                    }
                }
                GUI.Box(new Rect(200, Screen.height - 220, 90, 20), ((int)(scale*100))+"%", boxtexture);
            }
            else if (guisection == 1)
            {
                GUI.Box(new Rect(200, 0, 200, 25), "Ключевые события:", boxtexture);
                GUI.DrawTexture(new Rect(400, 0, 1, Screen.height - 200), graytexture);
                for (int i = 0; i < importantEvents.Count; i++) {
                    GUI.Box(new Rect(200,25+25*i,175,25),importantEvents[i].name,boxtexture);
                    if (GUI.Button(new Rect(375, 25 + 25 * i, 25, 25), trashpic, button)) {
                        importantEvents.Remove(importantEvents[i]);
                    }
                }
                if (GUI.Button(new Rect(200,25+ 25 * importantEvents.Count,200,25),"Добавить",button)) { addimportantevent = true; }
                if (GUI.Button(new Rect(400, 0, 300, 25), "Редактировать точку отсчёта", button)) {
                    creategreatpoint = true;
                }
                if (creategreatpoint) {
            //        if (GUI.Button(new Rect(400, 25, 300, 25), "Тип: " + greatpointtype, button)) { if (greatpointtype == greatepointtypes[1]) { greatpointtype = greatepointtypes[0]; } else { greatpointtype = greatepointtypes[1]; } }
                    GUI.Box(new Rect(400,25,300,50),"Название(в родительном падеже)",boxtexture);
                    greatpointname = GUI.TextField(new Rect(400,75,300,25),greatpointname,boxtexture);
                    GUI.Box(new Rect(400,100,250,25),timenamer+": "+greatpointpositionselector*timescaler,boxtexture);
                    if (GUI.Button(new Rect(650, 100, 25, 25), "0", button)) { greatpointpositionselector = 0; }
                    if (GUI.Button(new Rect(675, 100, 25, 25), dicepic, button)) { greatpointpositionselector = Random.Range(0,Screen.width); }
                    GUI.Box(new Rect(400,125,300,50),timenamer+" точки отсчёта указывается на таймлайне",boxtexture);
                    GUI.DrawTexture(new Rect(greatpointpositionselector,Screen.height-200,1,200),greentexture);
                    if (GUI.Button(new Rect(400, 175, 300, 25), "Установить точку отсчёта", button))
                    {
                        creategreatpoint = false;
                    }
                    if (GUI.Button(new Rect(400, 200, 300, 25), "Удалить точку отсчёта", button))
                    {
                        greatpointpositionselector = 0;
                        greatpointname = "";
                        creategreatpoint = false;
                    }
                }
                GUI.Box(new Rect(Screen.width-400,0,200,25),"Единица времени:",boxtexture);
                for (int i = 0; i < timenamers.Length; i++) {
                    if (GUI.Button(new Rect(Screen.width-400,25+ 25 * i, 200, 25), timenamers[i], button)) {
                        timenamer = timenamers[i];
                    }
                }
                GUI.Box(new Rect(Screen.width-400,25+25*timenamers.Length,200,25), "Единиц в делении:", boxtexture);
                timescaler = int.Parse(GUI.TextField(new Rect(Screen.width-400,50+25*timenamers.Length,200,25),timescaler.ToString(),boxtexture));
                
            }
            else if (guisection == 2)
            {
                for (int i = 0; i < textslist.Count; i++) {
                    if (GUI.Button(new Rect(Screen.width - 200, 25 * i, 200, 25), textslist[i], button)) {
                        if(textselector!=-1)SaveText(textselector);
                        textselector = i;
                        LoadText(i);
                    }
                }
                if (textselector != -1)
                {
                    GUI.Box(new Rect(200,0,Screen.width-400,Screen.height-200),"",boxtexture);
                    for (int i = 0; i < (Screen.height - 200) / 50; i++) if (linecounter + i < stringlist.Count)
                        {
                            stringlist[linecounter + i] = GUI.TextArea(new Rect(200, 50 * i, Screen.width - 400, 50), stringlist[linecounter + i], textfilebox);
                        }
                }
                if (GUI.Button(new Rect(Screen.width - 225, Screen.height - 225, 25, 25), "═", textbox)) {
                    if (textfilebox.alignment == TextAnchor.MiddleCenter)
                    {
                        textfilebox.alignment = TextAnchor.MiddleLeft;
                    }
                    else if (textfilebox.alignment == TextAnchor.MiddleLeft)
                    {
                        textfilebox.alignment = TextAnchor.MiddleRight;
                    }
                    else
                    {
                        textfilebox.alignment = TextAnchor.MiddleCenter;
                    }
                }
            }
            else if (guisection == 3)
            {
                if (cards.Count == 0 && !addcard)
                {
                    //    if (GUI.Button(new Rect(Screen.width*0.5f-150,Screen.height*0.4f-25,300,50),"Создать карточку",button)) { addcard = true; bufferindex.Add(0);selectedcardindex.Add(0);}
                }
                megacounter = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    show(cards[i], 0);
                }
                if (cards.Count > cardselector)
                {
                    if (!addcard) if (GUI.Button(new Rect(Screen.width - 500, 0, 300, 25), "Создать дочернюю карточку", button))
                        {
                            addcard = true;
                            addtype = 1;
                            bufferindex = selectedcardindex;
                        }
                }
                if (GUI.Button(new Rect(Screen.width - 225, Screen.height - 250, 25, 25), movepic, button))
                {
                    movenow = !movenow;
                }
                if (GUI.Button(new Rect(Screen.width - 225, Screen.height - 225, 25, 25), scrollpic, button)) {
                    showpresetlist = true;
                    LoadPresets();
                }
                //содержимое
                GUI.Box(new Rect((Screen.width) * 0.5f - 100, 0, 200, 30), "" + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).name, boxtexture);
                if(!showpresetlist&&!createpreset) for (int i = 0; i < GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Count; i++)
                {
                    if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].image)
                    {
                        if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture == null)
                        {
                            print(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text);
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture = new Texture2D(200, 200);
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture.LoadImage(File.ReadAllBytes(projectpath+"/pictures/"+GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text));
                        }
                        else
                        {
                            GUI.DrawTexture(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture);
                        }
                    }
                    else
                    {
                        if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].note)
                        {
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text = GUI.TextField(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text, paperbox);
                        }
                        else
                        {
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text = GUI.TextField(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text, boxtexture);
                        }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y - 10, 20, 20), dragpic, button))
                    {
                        if (!drag)
                        {
                            currentdrag = i;
                            drag = true;
                        }
                        else { drag = false; }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y + 10, 20, 20), horizontalscalepic, button))
                    {
                        if (scaletype != 1) { scaletype = 1; currentdrag = i; } else { scaletype = 0; }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y + 30, 20, 20), verticalscalepic, button))
                    {
                        if (scaletype != 2) { scaletype = 2; currentdrag = i; } else { scaletype = 0; }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y + 50, 20, 20), scalepic, button))
                    {
                        if (scaletype != 3) { scaletype = 3; currentdrag = i; } else { scaletype = 0; }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y + 70, 20, 20), sandclockpic, button))
                    {
                        if (timelinegroup)
                        {
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].time = timeselector;
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].timegroup = true;
                        }
                        else {
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].time = timezoneselector;
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].timezonetype = timezonetype;
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].timegroup = false;
                        }
                    }
                    if (GUI.Button(new Rect(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.x + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.width - 10, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect.y + 90, 20, 20), trashpic, button))
                    {
                        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Remove(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i]);
                    }
                        if (GUI.Button(new Rect(200, Screen.height - 225, 25, 25), trashpic, button)) {
                            for (int ii = 0; ii < selectedcardindex.Count - 1; ii++) {
                            bufferindex.Add(selectedcardindex[ii]);}
                            GetCard(cards[selectedcardindex[0]], bufferindex, 1).childrens.Remove(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1));
                            selectedcardindex = bufferindex;
                        }
                }
                //кнопки
                if (rightselector)
                {
                    if (GUI.Button(new Rect(point.x, point.y, 250, 25), "Добавить изображение", button))
                    {
                        addimage = true;
                        rightselector = false;
                    }
                    if (GUI.Button(new Rect(point.x, point.y + 25, 250, 25), "Добавить текст", button))
                    {
                        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Add(new Block(new Rect(point.x, point.y, 200, 400), "Новый текстовый блок"));
                        rightselector = false;
                    }
                    if (GUI.Button(new Rect(point.x, point.y + 50, 250, 25), "Добавить заметку", button)) {
                        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Add(new Block(new Rect(point.x, point.y, 200, 400), "Новая заметка",true));
                        rightselector = false;
                    }
                }
                //вывод шаблонов
                if (showpresetlist) {
                    GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,400),"",boxtexture);
                    GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,25),"Шаблоны разметки",boxtexture);
                    for (int i = 0; i < 14; i++) if(i<presetlist.Count){
                        if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 175 + i * 25, 375, 25), "" + presetlist[i], button)) {
                            ApplyPreset(i);
                            showpresetlist = false;
                        }
                            if (GUI.Button(new Rect(Screen.width * 0.5f + 175, Screen.height * 0.4f - 175 + i, 25, 25), trashpic, button)) {
                                DestroyPreset(i);
                            }
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 200, 25), "Отмена", button)) {
                        showpresetlist = false;
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f + 175, 200, 25), "Сохранить шаблон", button)) {
                        createpreset = true;
                        bufferstring = "";
                        showpresetlist = false;
                    }
                }
                //создание шаблонов
                if (createpreset)
                {
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 200, 400, 400), "", boxtexture);
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 200, 400, 25), "Добавить шаблон", boxtexture);
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 200, 25),"Название:",boxtexture);
                    bufferstring = GUI.TextField(new Rect(Screen.width*0.5f,Screen.height*0.4f-100,200,25),bufferstring,boxtexture);
                    if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 200, 25), "Отмена", button))
                    {
                        createpreset = false;
                    }
                    if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f + 175, 200, 25), "Сохранить шаблон", button))
                    {
                        AddPreset(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1),bufferstring);
                        createpreset = false;
                        bufferstring = "";
                    }
                }
            }
            else if (guisection == 4)
            {
                GUI.Box(new Rect(0, Screen.height * 0.3f + 125 + 25 * selectedselection, 200, 25), "", selectedbutton);
                megacounter = 0;
                show(cards[selectedselection],0);
                GUI.Box(new Rect((Screen.width) * 0.5f - 100, 0, 200, 30), "" + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).name, boxtexture);
                if (!showpresetlist && !createpreset) for (int i = 0; i < GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Count; i++)
                    {
                        if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].image)
                        {
                            if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture == null)
                            {
                                print(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text);
                                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture = new Texture2D(200, 200);
                                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture.LoadImage(File.ReadAllBytes(projectpath + "/pictures/" + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text));
                            }
                            else
                            {
                                GUI.DrawTexture(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].picture);
                            }
                        }
                        else
                        {
                            if (GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].note)
                            {
                                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text = GUI.TextField(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text, paperbox);
                            }
                            else
                            {
                                GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text = GUI.TextField(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].rect, GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks[i].text, boxtexture);
                            }
                        }
                    }
            }
            //добавить раздел
            if (addsection)
            {
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 200, 400, 400), "", boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 130, Screen.height * 0.4f - 200, 260, 25), "Добавление раздела", boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 150, 200, 25), "Название: ", boxtexture);
                bufferstring = GUI.TextField(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 150, 200, 25), bufferstring, boxtexture);
                if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f + 175, 200, 25), "Добавить раздел", button)) {
                    arbitraryselections.Add(bufferstring);
                    bufferindex = new List<int>();
                    bufferindex.Add(arbitraryselections.Count-1);
                    cards.Add(new Card(bufferstring,bufferindex));
                    bufferstring = "";
                    addsection = false;
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 200, 25), "Отмена", button)) { bufferstring = ""; addsection = false; }
            }
            //добавить карточку
            if (addcard)
            {
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 200, 400, 400), "", boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 125, Screen.height * 0.4f - 200, 250, 25), "Добавление карточки", boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 150, 200, 25), "Название: ", boxtexture);
                bufferstring = GUI.TextField(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 150, 200, 25), bufferstring, boxtexture);
                if (addtype == 0)
                {
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 200, 25), "Категория: ", boxtexture);
                    bufferindex[0] = GUI.SelectionGrid(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 75, 400, 100), bufferindex[0], arbitraryselections.ToArray(), 4, button);
                }
                else
                {
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 400, 50), "Карточка будет дочерней от " + GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).name, boxtexture);
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f - 0, Screen.height * 0.4f + 175, 200, 25), "Добавить карточку", button))
                {
                    if (addtype == 0)
                    {
                        cards.Add(new Card(bufferstring, bufferindex));
                        selectedcardindex = bufferindex;
                    }
                    else
                    {
                        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).childrens.Add(new Card(bufferstring, bufferindex));
                        GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).childrens[GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).childrens.Count - 1].index.Add(GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).childrens.Count - 1);
                    }
                    bufferstring = ""; addcard = false; bufferindex = new List<int>();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 200, 25), "Отмена", button)) { bufferstring = ""; addcard = false; bufferindex = new List<int>(); }

            }
            //добавить(загрузить) изображение
            if (addimage) {
                if (projectlist.Count > 0)
                {
                    for (int i = 0; i < projectlist.Count; i++)
                    {
                        if (GUI.Button(new Rect(point.x, point.y + 25 * i, 200, 25), projectlist[i], button))
                        {
                            GetCard(cards[selectedcardindex[0]], selectedcardindex, 1).blocks.Add(new Block(new Rect(point.x, point.y, 200, 200), null, projectlist[i]));
                            addimage = false;
                        }
                    }
                }
                else
                {
                    GUI.Box(new Rect(point.x, point.y, 400, 70), "Разместите .png-файлы в папке Setting Control_Data/projects/" + projectname + "/pictures/", boxtexture);
                    if (GUI.Button(new Rect(point.x+150,point.y+65,100,50),"Понятно",button)){
                        addimage = false;
                    }
                }
            }
            //добавить важное событие
            if (addimportantevent) {
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 200, 400, 400),"",boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 125, Screen.height * 0.4f - 200, 250, 25),"Новое ключевое событие",boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f-200, Screen.height * 0.4f - 150, 200, 25),"Название:",boxtexture);
                bufferstring = GUI.TextField(new Rect(Screen.width*0.5f,Screen.height*0.4f-150,200,25),bufferstring,boxtexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 125, 175, 25), "Цвет:", boxtexture);
                if(GUI.Button(new Rect(Screen.width * 0.5f - 25, Screen.height * 0.4f - 125, 25, 25), dicepic, button)){
                    if (Random.Range(0, 2) == 0)
                    {
                        buffercolor.r = Random.Range(0f, 1f);
                        buffercolor.g = buffercolor.r / 2;
                        buffercolor.b = buffercolor.r / 2;
                    }
                    else if (Random.Range(0, 2) == 0)
                    {
                        buffercolor.g = Random.Range(0f, 1f);
                        buffercolor.b = buffercolor.r / 2;
                        buffercolor.r = buffercolor.r / 2;
                    }
                    else
                    {
                        buffercolor.b = Random.Range(0f, 1f);
                        buffercolor.r = buffercolor.r / 2;
                        buffercolor.g = buffercolor.r / 2;
                    }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 125, 200, 25),buffercolortexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 200, 25),"R:"+(int)(buffercolor.r/0.0039f),boxtexture);
                if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 100, 25, 25), "+", button))
                {
                    if (buffercolor.r < 1f) { buffercolor.r += 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +25, Screen.height * 0.4f - 100, 50, 25), dicepic, button))
                {
                    buffercolor.r = Random.Range(0f, 1f);
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +75, Screen.height * 0.4f - 100, 25, 25), "-", button))
                {
                    if (buffercolor.r > 0.0039f) { buffercolor.r -= 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 75, 200, 25),"G:"+ (int)(buffercolor.g/0.0039f),boxtexture);
                if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 75, 25, 25), "+", button))
                {
                    if (buffercolor.g < 1f) { buffercolor.g += 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +25, Screen.height * 0.4f - 75, 50, 25), dicepic, button))
                {
                    buffercolor.g = Random.Range(0f, 1f);
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +75, Screen.height * 0.4f - 75, 25, 25), "-", button))
                {
                    if (buffercolor.g > 0.0039f) { buffercolor.g -= 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 50, 200, 25),"B:"+ (int)(buffercolor.b/0.0039f),boxtexture);
                if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f - 50, 25, 25), "+", button))
                {
                    if (buffercolor.b < 1f) { buffercolor.b += 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +25, Screen.height * 0.4f - 50, 50, 25), dicepic, button))
                {
                    buffercolor.b = Random.Range(0f, 1f);
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (GUI.Button(new Rect(Screen.width * 0.5f +75, Screen.height * 0.4f - 50, 25, 25), "-", button))
                {
                    if (buffercolor.b > 0.0039f) { buffercolor.b -= 0.0039f; }
                    buffercolortexture.SetPixel(0, 0, buffercolor);
                    buffercolortexture.Apply();
                }
                if (greatpointname == "")
                {
                    GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 400, 60), "Дата: " + (buffertimepos * timescaler) + " " + timenamer, boxtexture);
                }
                else
                {
                    if (buffertimepos < greatpointpositionselector)
                    {
                        GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 400, 60), "Дата: " + ((greatpointpositionselector-buffertimepos)* timescaler) + " " + timenamer+" до "+greatpointname, boxtexture);
                    }
                    else
                    {
                        GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 400, 60), "Дата: " + ((buffertimepos -greatpointpositionselector)* timescaler) + " " + timenamer+" от "+greatpointname, boxtexture);
                    }
                }
                GUI.DrawTexture(new Rect(buffertimepos,Screen.height-110,1,110),buffercolortexture);
                GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f+45, 400, 25), "Дата указывается в окне таймлайна", boxtexture);
                if (GUI.Button(new Rect(Screen.width * 0.5f, Screen.height * 0.4f + 175, 200, 25), "Добавить событие", button)) {
                importantEvents.Add(new ImportantEvent(bufferstring,buffertimepos,buffercolor));
                    importantEvents.Sort(delegate(ImportantEvent a, ImportantEvent b) {
                        return a.position.CompareTo(b.position);
                    });
                    bufferstring = ""; addimportantevent = false; }
                if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 200, 25), "Отмена", button)) { bufferstring = ""; addimportantevent=false; }
            }
        }
        else if (guiphase == 2) {
            GUI.Box(new Rect(Screen.width*0.5f-300,Screen.height*0.5f-200,600,200), "", boxtexture);
            GUI.Box(new Rect(Screen.width*0.5f-100,Screen.height*0.5f-200,200,25),"Новый проект",boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 250, Screen.height * 0.5f - 125, 200, 25), "Название:", boxtexture);
            if (GUI.Button(new Rect(Screen.width * 0.5f - 50, Screen.height * 0.5f - 125, 20, 25), dicepic, button)) {
                bufferstring = "";
                for (int i = 0; i < Random.Range(4, 12); i++) {
                    bufferstring += chars[Random.Range(0, chars.Length)];
                }
            }
            bufferstring = GUI.TextField(new Rect(Screen.width*0.5f-250,Screen.height*0.5f-100,200,25),bufferstring,boxtexture);
            if (GUI.Button(new Rect(Screen.width * 0.5f + 50, Screen.height * 0.5f - 100, 200, 25), "Создать", button)) {
                CreateProject();
                LoadPictures();
                guiphase = 1;
                bufferstring = "";
            }
        }
        if (GUI.Button(new Rect(0, 0, 200, 60), SettingControlLogo, button))
        {
            leftmenu = !leftmenu;
        }
        if (showinfo) {
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,400), "", boxtexture);
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,25), "О программе", boxtexture);
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-175,400,25), "Setting Control ver 1.03", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 150, 400, 50), "Информация, обновления и другие программы - vk.com/rival_coding", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 400, 25), "Разработчики: ", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 75, 400, 50), "Алексей Rival - буквально всё - vk.com/rival_alexey", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 400, 50), "Даниил(Фёдор) Красницкий - принятие решений и оценочное суждение", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 25,400, 25), "Благодарности:", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 50, 400, 75), "Imargo Morthis - консультации по сеттингам, помощь в пиаре - vk.com/invisiblesyro", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f +125, 400, 50), "Моя любимая мама - за всё светлое, что есть в моей жизни", boxtexture);
            if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 400, 25), "Понятно", button)) {
                showinfo = false;
            }
        }
        if (showhelp) {
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,400), "", boxtexture);
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-200,400,25), "Помощь", boxtexture);
            GUI.Box(new Rect(Screen.width*0.5f-200,Screen.height*0.4f-175,400,25), "Картографический режим:", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 150, 400, 50), "ПКМ+Пробел - включить режим перетаскивания", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 100, 400, 25), "+-/колёсико - приблизить/отдалить", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 75, 400, 50), "ЛКМ - установка точек и зон", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f - 25, 400, 50), "Ctrl+0 - сбросить масштаб", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 25,400, 25), "В остальных местах:", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 50, 400, 75), "ЛКМ - действие, ПКМ - контекстное меню(если есть)", boxtexture);
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f +125, 400, 50), "По остальным вопросам - vk.com/rival_coding", boxtexture);
            if (GUI.Button(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.4f + 175, 400, 25), "Понятно", button)) {
                showhelp = false;
            }
        }
    }
}
