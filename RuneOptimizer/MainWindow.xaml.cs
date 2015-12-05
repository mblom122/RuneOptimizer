using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Globalization;
namespace RuneOptimizer
{

    public class Optimized
    {
        public double Opt_ID { get; set; }
        public string Opt_RuneIDs { get; set; }
        public double Opt_ATK { get; set; }
        public double Opt_DEF { get; set; }
        public double Opt_HP { get; set; }
        public int Opt_CRate { get; set; }
        public int Opt_CDmg { get; set; }
        public int Opt_Acc { get; set; }
        public double Opt_Spd { get; set; }
        public int Opt_Res { get; set; }

        public Optimized(double id, string rIDS, double atk, double def, double hp, int crate, int cdmg, int acc, double spd, int res)
        {
            Opt_ID = id;
            Opt_RuneIDs = rIDS;
            Opt_ATK = atk;
            Opt_DEF = def;
            Opt_HP = hp;
            Opt_CRate = crate;
            Opt_CDmg = cdmg;
            Opt_Acc = acc;
            Opt_Spd = spd;
            Opt_Res = res;
        }
    }

    public class Rune_Set
    {
        public string RuneName { get; set; }
        public int RuneID { get; set; }
        public int Pices { get; set; }
        public int Bonus_Amount { get; set; }
        public RuneSets RuneTypes { get; set; }

        public enum RuneSets { Energy, Fatal, Blade, Swift, Focus, Guard, Endure, Shield, Revenge, Will, Nemesis, Vampire, Destroy, Despair, Violent, Rage};

        public Rune_Set()
        {
        }

        public Rune_Set(string n, int s)
        {
            RuneName = n;
            Bonus_Amount = s;
        }
    }

    public class ExportString
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ExportString(string n, string t)
        {
            Name = n;
            Type = t;
        }
    }

    public class Monster
    {
        public int Monster_ID { get; set; }
        public string Monster_Name { get; set; }
        public int Monster_Level { get; set; }
        public double Monster_Base_HP { get; set; }
        public double Monster_Base_Atk { get; set; }
        public double Monster_Base_Def { get; set; }
        public int Monster_Base_Spd { get; set; }
        public int Monster_Base_CritRate { get; set; }
        public int Monster_Base_CritDmg { get; set; }
        public int Monster_Base_Res { get; set; }
        public int Monster_Base_Acc { get; set; }
        public double Monster_Set_HP { get; set; }
        public double Monster_Set_Atk { get; set; }
        public double Monster_Set_Def { get; set; }
        public double Monster_Set_Spd { get; set; }
        public int Monster_Set_CritRate { get; set; }
        public int Monster_Set_CritDmg { get; set; }
        public int Monster_Set_Res { get; set; }
        public int Monster_Set_Acc { get; set; }
        public string Monster_RuneSet_1 { get; set; }
        public string Monster_RuneSet_2 { get; set; }
        public string Monster_RuneSet_3 { get; set; }
    }

    public class Rune
    {
        public int Rune_ID { get; set; }
        public int Rune_Monster { get; set; }
        public string Rune_Monster_Name { get; set; }
        public string Rune_Set { get; set; }
        public int Rune_Slot { get; set; }
        public int Rune_Grade { get; set; }
        public int Rune_Level { get; set; }
        public string Rune_Main_Type { get; set; }
        public int Rune_Main_Amount { get; set; }
        public string Rune_Innate_Type { get; set; }
        public int Rune_Innate_Amount { get; set; }
        public string Rune_Sub1_Type { get; set; }
        public int Rune_Sub1_Amount { get; set; }
        public string Rune_Sub2_Type { get; set; }
        public int Rune_Sub2_Amount { get; set; }
        public string Rune_Sub3_Type { get; set; }
        public int Rune_Sub3_Amount { get; set; }
        public string Rune_Sub4_Type { get; set; }
        public int Rune_Sub4_Amount { get; set; }
        public int Rune_Locked { get; set; }
        public int Rune_Sub_HP_F { get; set; }
        public int Rune_Sub_HP_P { get; set; }
        public int Rune_Sub_Atk_F { get; set; }
        public int Rune_Sub_Atk_P { get; set; }
        public int Rune_Sub_Def_F { get; set; }
        public int Rune_Sub_Def_P { get; set; }
        public int Rune_Sub_Spd { get; set; }
        public int Rune_Sub_CritRate { get; set; }
        public int Rune_Sub_CritDmg { get; set; }
        public int Rune_Sub_Res { get; set; }
        public int Rune_Sub_Acc { get; set; }
    }

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        List<Monster> Monsters = new List<Monster>();
        List<Rune> Runes = new List<Rune>();
        List<Rune_Set> SetPices = new List<Rune_Set>();
        List<Rune_Set> SetBonuses = new List<Rune_Set>();
        List<Optimized> OptimizeList = new List<Optimized>();
        List<string> TypeOfRuneSets = new List<string>();
        List<string> RuneLocations = new List<string> {"Inventory"};
        List<int> RuneGrades = new List<int>() { 1, 2, 3, 4, 5, 6 };
        List<int> RuneLevels = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        List<string> TypeOfRuneStats = new List<string>() { "ATK%", "ATK flat", "DEF%", "DEF flat", "HP%", "HP flat", "CRate", "CDmg", "ACC", "SPD", "RES"};
        List<string> TypeOfStatsSlot2 = new List<string>() { "ATK%", "ATK flat", "DEF%", "DEF flat", "HP%", "HP flat", "SPD" };
        List<string> TypeOfStatsSlot4 = new List<string>() { "ATK%", "ATK flat", "DEF%", "DEF flat", "HP%", "HP flat", "CRate", "CDmg" };
        List<string> TypeOfStatsSlot6 = new List<string>() { "ATK%", "ATK flat", "DEF%", "DEF flat", "HP%", "HP flat", "ACC", "RES" };
        List<string> selectedRuneSets = new List<string>();
        string selectedRuneSlot2 = "";
        string selectedRuneSlot4 = "";
        string selectedRuneSlot6 = "";
        Monster ref2Monster;
        DataTable tblOptimizer = new DataTable();
        double OptSearchCount = 0;

        public MainWindow()
        {
            CultureInfo culture;
            culture = CultureInfo.CreateSpecificCulture("en-US");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            InitializeComponent();

            MakeStartUpItems();
        }

        //Makes diffrent items and set binds etc, nessesary to work
        public void MakeStartUpItems()
        {
            string[] splitOne = new string[] { "\n" };
            string[] temp;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            string message = "Please make a \"\\Resources\\TypeOfRunes.txt\" in the application root dir. Make every rune set there is with: \"setname:nrofpices\". And for each new rune, make a new line.";
            string str = ReadTextFile(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\TypeOfRunes.txt", message);
            temp = str.Split(splitOne, StringSplitOptions.RemoveEmptyEntries);
            CreateLists(temp, "Rune_Sets");

            message = "Please make a \"\\Resources\\SetBonuses.txt\" in the application root dir. Make every rune set there is with: \"setname:setbonusamount\". And for each new rune, make a new line.";
            str = ReadTextFile(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\SetBonuses.txt", message);
            temp = str.Split(splitOne, StringSplitOptions.RemoveEmptyEntries);
            CreateLists(temp, "Set_Bonuses");

            CreateLists(temp, "TypeOfRuneSets");

            cmbRuneSlot.ItemsSource = RuneGrades;
            cmbRuneGrd.ItemsSource = RuneGrades;
            cmbRuneSet.ItemsSource = TypeOfRuneSets;
            cmbRuneMain.ItemsSource = TypeOfRuneStats;
            cmbRuneLvl.ItemsSource = RuneLevels;

            cmbOptMonsters.ItemsSource = Monsters;

            cmbOptSet1.ItemsSource = TypeOfRuneSets;
            cmbOptSet2.ItemsSource = TypeOfRuneSets;
            cmbOptSet3.ItemsSource = TypeOfRuneSets;

            cmbOptSlot2.ItemsSource = TypeOfStatsSlot2;
            cmbOptSlot4.ItemsSource = TypeOfStatsSlot4;
            cmbOptSlot6.ItemsSource = TypeOfStatsSlot6;

            try
            {
                // Try to delete the file.
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.sqlite");
            }
            catch (IOException)
            {
            }

            // Creates new sqlite database if it is not found
            using (var conn = new SQLiteConnection(
                @"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.sqlite"))
            {
                conn.Open();

                string sql = "create table OptSearch ( id double primary key, runeids string, atk double, def double, hp double, crate int, cdmg int, acc int, spd double, res int)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();

                conn.Close();
            }

            tblOptimizer.Columns.Add("id");
            tblOptimizer.Columns.Add("runeids");
            tblOptimizer.Columns.Add("atk");
            tblOptimizer.Columns.Add("def");
            tblOptimizer.Columns.Add("hp");
            tblOptimizer.Columns.Add("crate");
            tblOptimizer.Columns.Add("cdmg");
            tblOptimizer.Columns.Add("acc");
            tblOptimizer.Columns.Add("spd");
            tblOptimizer.Columns.Add("res");
            
            CreateDataGridData();
        }

        // run all background tasks here
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            OptSearchCount = 100000000;
            for (int i = 1; i < OptSearchCount; i++)
            {

                if (i % 1000 == 0)
                {
                    worker.ReportProgress((int)i);
                }

                Optimized temp = new Optimized(i, "123456", 100, 100, 100, 100, 100, 100, 100, 100);
                OptimizeList.Add(temp);
            }
            /*
            
            List<string> listCurrentSets = new List<string>();
            
            //Puts every rune in each runeslot in a list 
            IEnumerable<Rune> Slot_1 = from y in Runes where y.Rune_Slot == 1 select y;
            IEnumerable<Rune> Slot_2 = from y in Runes where y.Rune_Slot == 2 select y;
            IEnumerable<Rune> Slot_3 = from y in Runes where y.Rune_Slot == 3 select y;
            IEnumerable<Rune> Slot_4 = from y in Runes where y.Rune_Slot == 4 select y;
            IEnumerable<Rune> Slot_5 = from y in Runes where y.Rune_Slot == 5 select y;
            IEnumerable<Rune> Slot_6 = from y in Runes where y.Rune_Slot == 6 select y;
            IEnumerable<Rune> tempStoreRunes = from y in Runes where y.Rune_Monster_Name == ref2Monster.Monster_Name select y;

            Optimized optTempSearch;
            int modulo = 10000;
            double count;
            double countLoops = 0;
            double countMatch = 0;
            double timeETA = 0.0;
            bool setIsMatch;
            var stopwatch = new System.Diagnostics.Stopwatch();
            var time = TimeSpan.FromMilliseconds(timeETA);

            //Counts how many runes in each slot
            int rcount1 = Runes.Count(n => n.Rune_Slot == 1);
            int rcount2 = Runes.Count(n => n.Rune_Slot == 2);
            int rcount3 = Runes.Count(n => n.Rune_Slot == 3);
            int rcount4 = Runes.Count(n => n.Rune_Slot == 4);
            int rcount5 = Runes.Count(n => n.Rune_Slot == 5);
            int rcount6 = Runes.Count(n => n.Rune_Slot == 6);


            //Filters down rune list in slot 2,4 and 6 by selected main type and counts how many there is
            if (selectedRuneSlot2 != "")
            {
                Slot_2 = from y in Runes where y.Rune_Slot == 2 && y.Rune_Main_Type == selectedRuneSlot2 select y;
                rcount2 = Runes.Count(n => n.Rune_Slot == 2 && n.Rune_Main_Type == selectedRuneSlot2);
            }
            if (selectedRuneSlot4 != "")
            {
                Slot_4 = from y in Runes where y.Rune_Slot == 4 && y.Rune_Main_Type == selectedRuneSlot4 select y;
                rcount4 = Runes.Count(n => n.Rune_Slot == 4 && n.Rune_Main_Type == selectedRuneSlot4);
            }
            if (selectedRuneSlot6 != "")
            {
                Slot_6 = from y in Runes where y.Rune_Slot == 6 && y.Rune_Main_Type == selectedRuneSlot6 select y;
                rcount6 = Runes.Count(n => n.Rune_Slot == 6 && n.Rune_Main_Type == selectedRuneSlot6);
            }

            count = rcount1 * rcount2 * rcount3 * rcount4 * rcount5 * rcount6;

            OptSearchCount = count;

            //Starts timer
            stopwatch.Start();

            // Creates new sqlite database if it is not found
            using (var conn = new SQLiteConnection(
                @"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.sqlite"))
            {
                //Connects to database
                conn.Open();

                //Loops through all runes from every slot
                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var r1 in Slot_1)
                        {
                            foreach (var r2 in Slot_2)
                            {
                                foreach (var r3 in Slot_3)
                                {
                                    foreach (var r4 in Slot_4)
                                    {
                                        foreach (var r5 in Slot_5)
                                        {
                                            foreach (var r6 in Slot_6)
                                            {
                                                countLoops++;

                                                //Makes a list of the current sets that is made by runecombination
                                                listCurrentSets = CheckIfRuneSet(r1, r2, r3, r4, r5, r6);

                                                //Check if selectedRuneSets exists in the list listCurrentSets.
                                                //We only want to make a search on the selected runes
                                                setIsMatch = new HashSet<string>(listCurrentSets).IsSupersetOf(selectedRuneSets);

                                                if (setIsMatch == true)
                                                {
                                                    countMatch++;

                                                    //Gets the current stat of the monster using the current runes from loop                   
                                                    optTempSearch = SumStat(ref2Monster, r1, r2, r3, r4, r5, r6, countLoops);

                                                    //Makes a command that will be sent to data base later in commit
                                                    cmd.CommandText = "INSERT INTO OptSearch (id, runeids, atk, def, hp, crate, cdmg, acc, spd, res) VALUES (" + countLoops + ", '" + optTempSearch.Opt_RuneIDs + "', " + optTempSearch.Opt_ATK + ", " + optTempSearch.Opt_DEF + ", " + optTempSearch.Opt_HP + ", " + optTempSearch.Opt_CRate + ", " + optTempSearch.Opt_CDmg + ", " + optTempSearch.Opt_Acc + ", " + optTempSearch.Opt_Spd + ", " + optTempSearch.Opt_Res + ");";

                                                    //Executes database command
                                                    cmd.ExecuteNonQuery();
                                                }

                                                //Uses modulus to not opdate the progress to fast, not nessecary. 
                                                //Then reports the progress to the worker
                                                if (countLoops % modulo == 0)
                                                {
                                                    worker.ReportProgress((int)countLoops);

                                                    //Calculates estimated time
                                                    timeETA = stopwatch.Elapsed.TotalMilliseconds / countLoops;
                                                    time = TimeSpan.FromMilliseconds(timeETA * count);

                                                    //You cannot change the ui since its owned by the "main", so to by pass we do this
                                                    this.Dispatcher.BeginInvoke((Action)delegate ()
                                                    {
                                                        lblOptMatch.Content = "Matches: " + (int)countMatch;
                                                        lblOptETA.Content = time.ToString(@"dd\:hh\:mm\:ss");
                                                    });
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Saves to database
                        transaction.Commit();
                    }
                }

                //Closes database connection
                conn.Close();
            }

            UpdateMonsterStats(ref2Monster, tempStoreRunes);
            //xmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.xml");
            */
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            this.Dispatcher.BeginInvoke((Action)delegate () {
                progBarOptimizer.Value = 100;
            lblOptStatus.Content = OptSearchCount + " / " + OptSearchCount;
            lblOptETA.Content = "Done!";
            dgOptimize.Items.Refresh();
            });
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Updates ui to show progress
            this.Dispatcher.BeginInvoke((Action)delegate () {
                progBarOptimizer.Value = Math.Min((e.ProgressPercentage*100) / OptSearchCount, 100);
                lblOptStatus.Content = e.ProgressPercentage.ToString() + " / " + OptSearchCount;
            });
            //textBox.Text = e.ProgressPercentage.ToString();
        }

        public void CreateDataGridData()
        {      
            // Binds comboboxes in datagrid to lists   
            Rune_Set.ItemsSource = TypeOfRuneSets;
            Rune_Slot.ItemsSource = RuneGrades;
            Rune_Grade.ItemsSource = RuneGrades;
            Rune_Monster_Name.ItemsSource = RuneLocations;
            Rune_Main_Type.ItemsSource = TypeOfRuneStats;
            Rune_Innate_Type.ItemsSource = TypeOfRuneStats;
            Rune_Sub1_Type.ItemsSource = TypeOfRuneStats;
            Rune_Sub2_Type.ItemsSource = TypeOfRuneStats;
            Rune_Sub3_Type.ItemsSource = TypeOfRuneStats;
            Rune_Sub4_Type.ItemsSource = TypeOfRuneStats;

            //link runes list to CollectionViewSource
            CollectionViewSource itemCollectionViewSourceRune;
            itemCollectionViewSourceRune = (CollectionViewSource)(FindResource("ItemCollectionViewSourceRune"));
            itemCollectionViewSourceRune.Source = Runes;

            //link monster list to CollectionViewSource
            CollectionViewSource itemCollectionViewSourceMonster;
            itemCollectionViewSourceMonster = (CollectionViewSource)(FindResource("ItemCollectionViewSourceMonster"));
            itemCollectionViewSourceMonster.Source = Monsters;

            cmbRuneLoc.ItemsSource = RuneLocations;
        }

        public void CreateLists(string[] arrItem, string Type)
        {
            string[] splitCommonTwo = new string[] { "," };
            string[] splitCommonThree = new string[] { ":" };
            string[] temp1, temp2;
            bool error = false;

            //Splits up each item from array to get stats
            for (int i = 0; i < arrItem.Count(); i++)
            {

                // Creates new Monster and Rune to add to list
                Rune newRune = new Rune();
                Monster newMonster = new Monster();
                Rune_Set newRune_Set = new Rune_Set();

                // Deletes and replace " and - in string
                arrItem[i] = arrItem[i].Replace("\"", "");
                arrItem[i] = arrItem[i].Replace("-", "0");

                try
                {
                    if (Type == "Runes" || Type == "Monsters")
                    {
                        // Splits to get Ex: "id":1
                        temp1 = arrItem[i].Split(splitCommonTwo, StringSplitOptions.RemoveEmptyEntries);

                        for (int n = 0; n < temp1.Count(); n++)
                        {
                            // Splits to only get 1 from "id":1
                            temp2 = temp1[n].Split(splitCommonThree, StringSplitOptions.None);

                            // Saves stats into new monster / rune
                            if (Type == "Monsters")
                            {
                                switch (temp2[0])
                                {
                                    case "id":
                                        newMonster.Monster_ID = int.Parse(temp2[1]);
                                        break;
                                    case "name":
                                        newMonster.Monster_Name = temp2[1];
                                        break;
                                    case "level":
                                        newMonster.Monster_Level = int.Parse(temp2[1]);
                                        break;
                                    case "b_hp":
                                        newMonster.Monster_Base_HP = int.Parse(temp2[1]);
                                        break;
                                    case "b_atk":
                                        newMonster.Monster_Base_Atk = int.Parse(temp2[1]);
                                        break;
                                    case "b_def":
                                        newMonster.Monster_Base_Def = int.Parse(temp2[1]);
                                        break;
                                    case "b_spd":
                                        newMonster.Monster_Base_Spd = int.Parse(temp2[1]);
                                        break;
                                    case "b_crate":
                                        newMonster.Monster_Base_CritRate = int.Parse(temp2[1]);
                                        break;
                                    case "b_cdmg":
                                        newMonster.Monster_Base_CritDmg = int.Parse(temp2[1]);
                                        break;
                                    case "b_res":
                                        newMonster.Monster_Base_Res = int.Parse(temp2[1]);
                                        break;
                                    case "b_acc":
                                        newMonster.Monster_Base_Acc = int.Parse(temp2[1]);
                                        break;
                                    default:
                                        error = true;
                                        break;
                                }
                            }
                            else if (Type == "Runes")
                            {
                                switch (temp2[0])
                                {
                                    case "id":
                                        newRune.Rune_ID = int.Parse(temp2[1]);
                                        break;
                                    case "monster":
                                        newRune.Rune_Monster = int.Parse(temp2[1]);
                                        break;
                                    case "monster_n":
                                        newRune.Rune_Monster_Name = temp2[1];
                                        break;
                                    case "set":
                                        newRune.Rune_Set = temp2[1];
                                        break;
                                    case "slot":
                                        newRune.Rune_Slot = int.Parse(temp2[1]);
                                        break;
                                    case "grade":
                                        newRune.Rune_Grade = int.Parse(temp2[1]);
                                        break;
                                    case "level":
                                        newRune.Rune_Level = int.Parse(temp2[1]);
                                        break;
                                    case "m_t":
                                        newRune.Rune_Main_Type = temp2[1];
                                        break;
                                    case "m_v":
                                        newRune.Rune_Main_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "i_t":
                                        newRune.Rune_Innate_Type = temp2[1];
                                        break;
                                    case "i_v":
                                        newRune.Rune_Innate_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "s1_t":
                                        newRune.Rune_Sub1_Type = temp2[1];
                                        break;
                                    case "s1_v":
                                        newRune.Rune_Sub1_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "s2_t":
                                        newRune.Rune_Sub2_Type = temp2[1];
                                        break;
                                    case "s2_v":
                                        newRune.Rune_Sub2_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "s3_t":
                                        newRune.Rune_Sub3_Type = temp2[1];
                                        break;
                                    case "s3_v":
                                        newRune.Rune_Sub3_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "s4_t":
                                        newRune.Rune_Sub4_Type = temp2[1];
                                        break;
                                    case "s4_v":
                                        newRune.Rune_Sub4_Amount = int.Parse(temp2[1]);
                                        break;
                                    case "locked":
                                        newRune.Rune_Locked = int.Parse(temp2[1]);
                                        break;
                                    case "sub_hpf":
                                        newRune.Rune_Sub_HP_F = int.Parse(temp2[1]);
                                        break;
                                    case "sub_hpp":
                                        newRune.Rune_Sub_HP_P = int.Parse(temp2[1]);
                                        break;
                                    case "sub_atkf":
                                        newRune.Rune_Sub_Atk_F = int.Parse(temp2[1]);
                                        break;
                                    case "sub_atkp":
                                        newRune.Rune_Sub_Atk_P = int.Parse(temp2[1]);
                                        break;
                                    case "sub_deff":
                                        newRune.Rune_Sub_Def_F = int.Parse(temp2[1]);
                                        break;
                                    case "sub_defp":
                                        newRune.Rune_Sub_Def_P = int.Parse(temp2[1]);
                                        break;
                                    case "sub_spd":
                                        newRune.Rune_Sub_Spd = int.Parse(temp2[1]);
                                        break;
                                    case "sub_crate":
                                        newRune.Rune_Sub_CritRate = int.Parse(temp2[1]);
                                        break;
                                    case "sub_cdmg":
                                        newRune.Rune_Sub_CritDmg = int.Parse(temp2[1]);
                                        break;
                                    case "sub_res":
                                        newRune.Rune_Sub_Res = int.Parse(temp2[1]);
                                        break;
                                    case "sub_acc":
                                        newRune.Rune_Sub_Acc = int.Parse(temp2[1]);
                                        break;
                                    default:
                                        error = true;
                                        break;

                                }
                            }
                        }
                    }
                    //Saves each runeset in list
                    else if (Type == "Rune_Sets")
                    {
                        temp1 = arrItem[i].Split(splitCommonThree, StringSplitOptions.RemoveEmptyEntries);
                        newRune_Set.RuneName = temp1[0];
                        newRune_Set.Pices = int.Parse(temp1[1]);
                        SetPices.Add(newRune_Set);
                    }
                    //Saves each type of rune and setbonuses into lists
                    else if (Type == "Set_Bonuses")
                    {
                        temp1 = arrItem[i].Split(splitCommonThree, StringSplitOptions.RemoveEmptyEntries);
                        newRune_Set.RuneName = temp1[0];
                        newRune_Set.Bonus_Amount = int.Parse(temp1[1]);
                        SetBonuses.Add(newRune_Set);
                        TypeOfRuneSets.Add(temp1[0]);
                    }

                    if (Type == "Runes"  && error == false)
                    {

                        Runes.Add(newRune);
                    }
                        

                    else if (Type == "Monsters" && error == false)
                        Monsters.Add(newMonster);
                }
                catch
                {
                     
                }
            }
            foreach (Monster x in Monsters)
            {
                CountSetPices(x);
            }
        }

        public void CountSetPices(Monster x)
        {
            int temp;
            foreach (Rune_Set y in SetPices)
            {
                //Counts each rune to determine if monster have set
                try
                {
                    temp = Runes.Count(n => n.Rune_Monster_Name == x.Monster_Name && n.Rune_Set == y.RuneName) / y.Pices;
                }
                catch (DivideByZeroException)
                {
                    temp = 0;
                }
                
                for (int i = 0; i < temp; i++)
                {
                    if (x.Monster_RuneSet_1 == null)
                        x.Monster_RuneSet_1 = y.RuneName;
                    else if (x.Monster_RuneSet_2 == null)
                        x.Monster_RuneSet_2 = y.RuneName;
                    else if (x.Monster_RuneSet_3 == null)
                        x.Monster_RuneSet_3 = y.RuneName;
                }
            }
        }

        public void Import(string text)
        {
            string[] splitMonOne = new string[] { "\"mons\":[{" };
            string[] splitMonTwo = new string[] { "}]" };
            string[] splitCommonOne = new string[] { "},{" };
            string[] splitRuneOne = new string[] { "{\"runes\":[{" };
            string[] splitRuneTwo = new string[] { "}]" };
            string[] temp;
            List<string> attList = new List<string>();
            bool error = false;
            
            try
            {
                // Rune list
                temp = text.Split(splitRuneOne, StringSplitOptions.RemoveEmptyEntries);
                temp = temp[0].Split(splitRuneTwo, StringSplitOptions.RemoveEmptyEntries);
                CreateLists(temp[0].Split(splitCommonOne, StringSplitOptions.RemoveEmptyEntries), "Runes");

                // Monster list
                temp = text.Split(splitMonOne, StringSplitOptions.RemoveEmptyEntries);
                temp = temp[1].Split(splitMonTwo, StringSplitOptions.RemoveEmptyEntries);
                CreateLists(temp[0].Split(splitCommonOne, StringSplitOptions.RemoveEmptyEntries), "Monsters");
            }
            catch (IndexOutOfRangeException)
            {
                error = true;
            }

            UpdateMonsters();
            //CreateDataGridData();
            
            dgMonsters.Items.Refresh();
            dgRunes.Items.Refresh();

            txtString.Text = "";
            if (error == false)
                lblImpExpStatus.Content = "Import done!";
            else
                lblImpExpStatus.Content = "Something went wrong!";
        }

        public void UpdateMonsters()
        {
            foreach (Monster x in Monsters)
            {
                IEnumerable<Rune> tempRunes = from y in Runes where y.Rune_Monster_Name == x.Monster_Name select y;
                UpdateMonsterStats(x,tempRunes);
                RuneLocations.Add(x.Monster_Name);
            }
        }

        public void UpdateMonsterStats(Monster x, IEnumerable<Rune> tempRunes)
        {
            // Sets all stats to 0
            x.Monster_Set_Atk = x.Monster_Set_Def = x.Monster_Set_HP = x.Monster_Set_Spd = x.Monster_Set_Acc = x.Monster_Set_CritDmg = x.Monster_Set_CritRate = x.Monster_Set_Res = 0;

            //Checks if rune is on mob
            foreach (Rune y in tempRunes)
            {
                //Setts stats on mob
                x.Monster_Set_Atk = x.Monster_Set_Atk + y.Rune_Sub_Atk_F + (x.Monster_Base_Atk * (y.Rune_Sub_Atk_P / 100.0));
                x.Monster_Set_Def = x.Monster_Set_Def + y.Rune_Sub_Def_F + (x.Monster_Base_Def * (y.Rune_Sub_Def_P / 100.0));
                x.Monster_Set_HP = x.Monster_Set_HP + y.Rune_Sub_HP_F + (x.Monster_Base_HP * (y.Rune_Sub_HP_P / 100.0));
                x.Monster_Set_Acc = x.Monster_Set_Acc + y.Rune_Sub_Acc;
                x.Monster_Set_CritDmg = x.Monster_Set_CritDmg + y.Rune_Sub_CritDmg;
                x.Monster_Set_CritRate = x.Monster_Set_CritRate + y.Rune_Sub_CritRate;
                x.Monster_Set_Spd = x.Monster_Set_Spd + y.Rune_Sub_Spd;
                x.Monster_Set_Res = x.Monster_Set_Res + y.Rune_Sub_Res;

                if (y.Rune_Main_Type == "ATK%")
                    x.Monster_Set_Atk = x.Monster_Set_Atk + (x.Monster_Base_Atk * (y.Rune_Main_Amount / 100.0));
                else if (y.Rune_Main_Type == "ATK flat")
                    x.Monster_Set_Atk = x.Monster_Set_Atk + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "DEF%")
                    x.Monster_Set_Def = x.Monster_Set_Def + (x.Monster_Base_Def * (y.Rune_Main_Amount / 100.0));
                else if (y.Rune_Main_Type == "DEF flat")
                    x.Monster_Set_Def = x.Monster_Set_Def + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "HP%")
                    x.Monster_Set_HP = x.Monster_Set_HP + (x.Monster_Base_HP * (y.Rune_Main_Amount / 100.0));
                else if (y.Rune_Main_Type == "HP flat")
                    x.Monster_Set_HP = x.Monster_Set_HP + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "CRate")
                    x.Monster_Set_CritRate = x.Monster_Set_CritRate + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "CDmg")
                    x.Monster_Set_CritDmg = x.Monster_Set_CritDmg + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "ACC")
                    x.Monster_Set_Acc = x.Monster_Set_Acc + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "SPD")
                    x.Monster_Set_Spd = x.Monster_Set_Spd + y.Rune_Main_Amount;
                else if (y.Rune_Main_Type == "RES")
                    x.Monster_Set_Res = x.Monster_Set_Res + y.Rune_Main_Amount;

                if (y.Rune_Innate_Type == "ATK%")
                    x.Monster_Set_Atk = x.Monster_Set_Atk + (x.Monster_Base_Atk * (y.Rune_Innate_Amount / 100.0));
                else if (y.Rune_Innate_Type == "ATK flat")
                    x.Monster_Set_Atk = x.Monster_Set_Atk + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "DEF%")
                    x.Monster_Set_Def = x.Monster_Set_Def + (x.Monster_Base_Def * (y.Rune_Innate_Amount / 100.0));
                else if (y.Rune_Innate_Type == "DEF flat")
                    x.Monster_Set_Def = x.Monster_Set_Def + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "HP%")
                    x.Monster_Set_HP = x.Monster_Set_HP + (x.Monster_Base_HP * (y.Rune_Innate_Amount / 100.0));
                else if (y.Rune_Innate_Type == "HP flat")
                    x.Monster_Set_HP = x.Monster_Set_HP + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "CRate")
                    x.Monster_Set_CritRate = x.Monster_Set_CritRate + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "CDmg")
                    x.Monster_Set_CritDmg = x.Monster_Set_CritDmg + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "ACC")
                    x.Monster_Set_Acc = x.Monster_Set_Acc + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "SPD")
                    x.Monster_Set_Spd = x.Monster_Set_Spd + y.Rune_Innate_Amount;
                else if (y.Rune_Innate_Type == "RES")
                    x.Monster_Set_Res = x.Monster_Set_Res + y.Rune_Innate_Amount;
            }

            if (x.Monster_RuneSet_1 == "Fatal" || x.Monster_RuneSet_2 == "Fatal" || x.Monster_RuneSet_3 == "Fatal")
            {
                var listval = from y in SetBonuses where y.RuneName == "Fatal" select y;
                x.Monster_Set_Atk = x.Monster_Set_Atk + (x.Monster_Base_Atk * (listval.First().Bonus_Amount / 100.0));
            }
            else if (x.Monster_RuneSet_1 == "Rage" || x.Monster_RuneSet_2 == "Rage" || x.Monster_RuneSet_3 == "Rage")
            {
                var listval = from y in SetBonuses where y.RuneName == "Rage" select y;
                x.Monster_Set_CritDmg = x.Monster_Set_CritDmg + listval.First().Bonus_Amount;
            }
            else if (x.Monster_RuneSet_1 == "Swift" || x.Monster_RuneSet_2 == "Swift" || x.Monster_RuneSet_3 == "Swift")
            {
                var listval = from y in SetBonuses where y.RuneName == "Swift" select y;
                x.Monster_Set_Spd = x.Monster_Set_Spd + (x.Monster_Base_Spd * (listval.First().Bonus_Amount / 100.0));
            }

            x.Monster_Set_HP = x.Monster_Set_HP + (x.Monster_Base_HP * (CountRuneSetBonus(x, "Energy")) / 100.0);
            x.Monster_Set_Def = x.Monster_Set_Def + (x.Monster_Base_Def * (CountRuneSetBonus(x, "Guard")) / 100.0);
            x.Monster_Set_CritRate = x.Monster_Set_CritRate + CountRuneSetBonus(x, "Blade");
            x.Monster_Set_Acc = x.Monster_Set_Acc + CountRuneSetBonus(x, "Focus");
            x.Monster_Set_Res = x.Monster_Set_Res + CountRuneSetBonus(x, "Endure");
            
        }

        public int CountRuneSetBonus(Monster x, string s)
        {
            int index = SetBonuses.Select((item, i) => new { Item = item, Index = i })
                                        .First(y => y.Item.RuneName == s).Index;

            if (x.Monster_RuneSet_1 == s && x.Monster_RuneSet_2 == s && x.Monster_RuneSet_3 == s)
                return (3 * SetBonuses[index].Bonus_Amount);
            else if (x.Monster_RuneSet_1 == s && x.Monster_RuneSet_2 == s || x.Monster_RuneSet_1 == s && x.Monster_RuneSet_3 == s || x.Monster_RuneSet_2 == s && x.Monster_RuneSet_3 == s)
                return (2 * SetBonuses[index].Bonus_Amount);
            else if (x.Monster_RuneSet_1 == s || x.Monster_RuneSet_2 == s || x.Monster_RuneSet_3 == s)
                return (SetBonuses[index].Bonus_Amount);
            else
                return (0);
        }

        public List<string> CheckIfRuneSet(Rune r1, Rune r2, Rune r3, Rune r4, Rune r5, Rune r6)
        {
            List<string> compRunes = new List<string>() {r1.Rune_Set, r2.Rune_Set , r3.Rune_Set , r4.Rune_Set , r5.Rune_Set , r6.Rune_Set };
            int count = 0;
            List<string> lstCurrentSets = new List<string>();

            foreach (var x in SetPices)
            {
                count = compRunes.Count(n => n == x.RuneName);

                if ((int)(count / x.Pices) == 3)
                {
                    lstCurrentSets.Add(x.RuneName);
                    lstCurrentSets.Add(x.RuneName);
                    lstCurrentSets.Add(x.RuneName);
                }
                else if ((int)(count / x.Pices) == 2)
                {
                    lstCurrentSets.Add(x.RuneName);
                    lstCurrentSets.Add(x.RuneName);
                }
                else if ((int)(count / x.Pices) == 1)
                {
                    lstCurrentSets.Add(x.RuneName);
                }
            }
            
            return (lstCurrentSets);
        }

        public void UpdateMonsterUI()
        {
            try
            {
                Monster monster = (Monster)dgMonsters.SelectedItem;

                //var listval = from x in Monsters where x.Monster_Name == dgMonsters.SelectedItem.ToString() select x;

                lblMonID.Content = monster.Monster_ID.ToString();
                lblMonName.Content = monster.Monster_Name.ToString();
                lblMonLevel.Content = monster.Monster_Level.ToString();
                lblMonRuneSet_1.Content = monster.Monster_RuneSet_1;
                lblMonRuneSet_2.Content = monster.Monster_RuneSet_2;
                lblMonRuneSet_3.Content = monster.Monster_RuneSet_3;
                lblMonBaseHp.Content = monster.Monster_Base_HP.ToString();
                lblMonBaseAtk.Content = monster.Monster_Base_Atk.ToString();
                lblMonBaseDef.Content = monster.Monster_Base_Def.ToString();
                lblMonBaseSpd.Content = monster.Monster_Base_Spd.ToString();
                lblMonBaseCRate.Content = monster.Monster_Base_CritRate.ToString();
                lblMonBaseCDmg.Content = monster.Monster_Base_CritDmg.ToString();
                lblMonBaseRes.Content = monster.Monster_Base_Res.ToString();
                lblMonBaseAcc.Content = monster.Monster_Base_Acc.ToString();

                lblMonSetHp.Content = Math.Round(monster.Monster_Set_HP).ToString();
                lblMonSetAtk.Content = Math.Round(monster.Monster_Set_Atk).ToString();
                lblMonSetDef.Content = Math.Round(monster.Monster_Set_Def).ToString();
                lblMonSetSpd.Content = Math.Round(monster.Monster_Set_Spd).ToString();
                lblMonSetCRate.Content = monster.Monster_Set_CritRate.ToString();
                lblMonSetCDmg.Content = monster.Monster_Set_CritDmg.ToString();
                lblMonSetRes.Content = monster.Monster_Set_Res.ToString();
                lblMonSetAcc.Content = monster.Monster_Set_Acc.ToString();

                lblMonTotHp.Content = Math.Round((monster.Monster_Set_HP + monster.Monster_Base_HP)).ToString();
                lblMonTotAtk.Content = Math.Round((monster.Monster_Set_Atk + monster.Monster_Base_Atk)).ToString();
                lblMonTotDef.Content = Math.Round((monster.Monster_Set_Def + monster.Monster_Base_Def)).ToString();
                lblMonTotSpd.Content = Math.Round((monster.Monster_Set_Spd + monster.Monster_Base_Spd)).ToString();
                lblMonTotCRate.Content = (monster.Monster_Set_CritRate + monster.Monster_Base_CritRate).ToString();
                lblMonTotCDmg.Content = (monster.Monster_Set_CritDmg + monster.Monster_Base_CritDmg).ToString();
                lblMonTotRes.Content = (monster.Monster_Set_Res + monster.Monster_Base_Res).ToString();
                lblMonTotAcc.Content = (monster.Monster_Set_Acc + monster.Monster_Base_Acc).ToString();
            }
            catch { }
            
        }

        private void dgMonsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMonsterUI();
        }

        public string ReadTextFile(string filepath, string message)
        {
            if (System.IO.File.Exists(filepath) == true)
            {
                System.IO.StreamReader objReader;
                objReader = new System.IO.StreamReader(filepath);
                string str = objReader.ReadToEnd();
                objReader.Close();
                return (str);
            }
            else
            {
                System.Windows.MessageBox.Show(message);
                return ("");
            }
        }

        private void butImportFile_Click(object sender, RoutedEventArgs e)
        {
            string message = "File does not exist!";
            //Import(ReadTextFile(txtFilePath.Text, message));
            Import(ReadTextFile(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\test.txt", message));

        }

        private void butImportString_Click(object sender, RoutedEventArgs e)
        {
            Import(txtString.Text);
        }

        private void butImportHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("There are 2 textboxes, one that imports string from textfile and one that imports from copybord. Just copy the export string and insert it into the string textbox and click import from string. The imported data can use the same as from the online \"Summoners War Rune Database and Optimizer\".");
        }

        private void butExportFile_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void butExportString_Click(object sender, RoutedEventArgs e)
        {
            string exportstring = MakeExportString();
            txtString.Text = exportstring;
            System.Windows.Forms.Clipboard.SetText(exportstring);

            lblImpExpStatus.Content = "Export done! String copied to clipboard.";
        }

        public string MakeExportString()
        {
            List<ExportString> ExportList = new List<ExportString>();
            ExportList.Add(new ExportString("id", "i"));
            ExportList.Add(new ExportString("monster", "i"));
            ExportList.Add(new ExportString("monster_n", "s"));
            ExportList.Add(new ExportString("set", "s"));
            ExportList.Add(new ExportString("slot", "i"));
            ExportList.Add(new ExportString("grade", "i"));
            ExportList.Add(new ExportString("level", "i"));
            ExportList.Add(new ExportString("m_t", "s"));
            ExportList.Add(new ExportString("m_v", "i"));
            ExportList.Add(new ExportString("i_t", "s"));
            ExportList.Add(new ExportString("i_v", "i"));
            ExportList.Add(new ExportString("s1_t", "s"));
            ExportList.Add(new ExportString("s1_v", "i"));
            ExportList.Add(new ExportString("s2_t", "s"));
            ExportList.Add(new ExportString("s2_v", "i"));
            ExportList.Add(new ExportString("s3_t", "s"));
            ExportList.Add(new ExportString("s3_v", "i"));
            ExportList.Add(new ExportString("s4_t", "s"));
            ExportList.Add(new ExportString("s4_v", "i"));
            ExportList.Add(new ExportString("locked", "s"));
            ExportList.Add(new ExportString("sub_crate", "s"));
            ExportList.Add(new ExportString("sub_acc", "s"));
            ExportList.Add(new ExportString("sub_defp", "s"));
            ExportList.Add(new ExportString("sub_atkp", "s"));
            ExportList.Add(new ExportString("sub_atkf", "s"));
            ExportList.Add(new ExportString("sub_deff", "s"));
            ExportList.Add(new ExportString("sub_hpp", "s"));
            ExportList.Add(new ExportString("sub_hpf", "s"));
            ExportList.Add(new ExportString("sub_spd", "s"));
            ExportList.Add(new ExportString("sub_res", "s"));
            ExportList.Add(new ExportString("sub_cdmg", "s"));

            string str = "{\"runes\":[";
            int monsterid, index;

            for (int i = 0;i < Runes.Count;i++)
            {
                index = Monsters.FindIndex(f => f.Monster_Name == Runes[i].Rune_Monster_Name);

                if (index >= 0)
                    monsterid = Monsters[index].Monster_ID;
                else
                    monsterid = 0;
                
                    str = str +
                    "{\"id\":" + Runes[i].Rune_ID + 
                    ",\"monster\":" + monsterid +
                    ",\"monster_n\":\"" + Runes[i].Rune_Monster_Name +
                    "\",\"set\":\"" + Runes[i].Rune_Set +
                    "\",\"slot\":" + Runes[i].Rune_Slot +
                    ",\"grade\":" + Runes[i].Rune_Grade +
                    ",\"level\":" + Runes[i].Rune_Level +
                    ",\"m_t\":\"" + Runes[i].Rune_Main_Type +
                    "\",\"m_v\":" + Runes[i].Rune_Main_Amount +
                    ",\"i_t\":\"" + Runes[i].Rune_Innate_Type +
                    "\",\"i_v\":" + Runes[i].Rune_Innate_Amount +
                    ",\"s1_t\":\"" + Runes[i].Rune_Sub1_Type +
                    "\",\"s1_v\":" + Runes[i].Rune_Sub1_Amount +
                    ",\"s2_t\":\"" + Runes[i].Rune_Sub2_Type +
                    "\",\"s2_v\":" + Runes[i].Rune_Sub2_Amount +
                    ",\"s3_t\":\"" + Runes[i].Rune_Sub3_Type +
                    "\",\"s3_v\":" + Runes[i].Rune_Sub3_Amount +
                    ",\"s4_t\":\"" + Runes[i].Rune_Sub4_Type +
                    "\",\"s4_v\":" + Runes[i].Rune_Sub4_Amount +
                    ",\"locked\":" + Runes[i].Rune_Locked +
                    ",\"sub_crate\":" + CheckIfEmpty(Runes[i].Rune_Sub_CritRate) +
                    ",\"sub_acc\":" + CheckIfEmpty(Runes[i].Rune_Sub_Acc) +
                    ",\"sub_defp\":" + CheckIfEmpty(Runes[i].Rune_Sub_Def_P) +
                    ",\"sub_atkp\":" + CheckIfEmpty(Runes[i].Rune_Sub_Atk_P) +
                    ",\"sub_atkf\":" + CheckIfEmpty(Runes[i].Rune_Sub_Atk_F) +
                    ",\"sub_deff\":" + CheckIfEmpty(Runes[i].Rune_Sub_Def_F) +
                    ",\"sub_hpp\":" + CheckIfEmpty(Runes[i].Rune_Sub_HP_P) +
                    ",\"sub_hpf\":" + CheckIfEmpty(Runes[i].Rune_Sub_HP_F) +
                    ",\"sub_spd\":" + CheckIfEmpty(Runes[i].Rune_Sub_Spd) +
                    ",\"sub_res\":" + CheckIfEmpty(Runes[i].Rune_Sub_Res) +
                    ",\"sub_cdmg\":" + CheckIfEmpty(Runes[i].Rune_Sub_CritDmg) + "}";

                if (i < Runes.Count - 1)
                    str = str + ",";
            }
            str = str + "],\"mons\":[";

            for (int i = 0; i < Monsters.Count; i++)
            {
                str = str +
                "{\"id\":" + Monsters[i].Monster_ID +
                ",\"name\":\"" + Monsters[i].Monster_Name +
                "\",\"level\":" + Monsters[i].Monster_Level +
                ",\"b_hp\":" + Monsters[i].Monster_Base_HP +
                ",\"b_atk\":" + Monsters[i].Monster_Base_Atk +
                ",\"b_def\":" + Monsters[i].Monster_Base_Def +
                ",\"b_spd\":" + Monsters[i].Monster_Base_Spd +
                ",\"b_crate\":" + Monsters[i].Monster_Base_CritRate +
                ",\"b_cdmg\":" + Monsters[i].Monster_Base_CritDmg +
                ",\"b_res\":" + Monsters[i].Monster_Base_Res +
                ",\"b_acc\":" + Monsters[i].Monster_Base_Acc + "}";

                if (i < Monsters.Count - 1)
                    str = str + ",";
            }

                str = str + "]}";
            return (str);
        }

        public string CheckIfEmpty(int i)
        {
            if (i < 1)
                return ("\"-\"");
            else
                return (i.ToString());
        }
        
        private void EnterClicked(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var _itemSourceList = new CollectionViewSource() { Source = Runes };
                ICollectionView Itemlist = _itemSourceList.View;

                // your Filter
                var yourCostumFilter = new Predicate<object>(item =>
                {
                    var rune = item as Rune;
                    return item == null || rune.Rune_Monster_Name.Contains(txtRuneFilter.Text) || rune.Rune_Set.Contains(txtRuneFilter.Text) || Regex.IsMatch(rune.Rune_ID.ToString(), txtRuneFilter.Text) || Regex.IsMatch(rune.Rune_Slot.ToString(), txtRuneFilter.Text) || Regex.IsMatch(rune.Rune_Grade.ToString(), txtRuneFilter.Text) || Regex.IsMatch(rune.Rune_Level.ToString(), txtRuneFilter.Text) || rune.Rune_Main_Type.Contains(txtRuneFilter.Text);
                });
               
                //now we add our Filter
                Itemlist.Filter = yourCostumFilter;

                dgRunes.ItemsSource = Itemlist;

                e.Handled = true;
            }
        }

        private void cmbRuneSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DGSortRunes();

            e.Handled = true;
        }

        private void DGSortRunes()
        {
            var _itemSourceList = new CollectionViewSource() { Source = Runes };
            ICollectionView Itemlist = _itemSourceList.View;

            // your Filter
            var yourCostumFilter = new Predicate<object>(item =>
            {
                var rune = item as Rune;
                return rune.Rune_Monster_Name.Contains(cmbRuneLoc.Text) && rune.Rune_Set.Contains(cmbRuneSet.Text) && Regex.IsMatch(rune.Rune_Slot.ToString(), cmbRuneSlot.Text) && Regex.IsMatch(rune.Rune_Grade.ToString(), cmbRuneGrd.Text) && Regex.IsMatch(rune.Rune_Level.ToString(), cmbRuneLvl.Text) && rune.Rune_Main_Type.Contains(cmbRuneMain.Text);
            });

            //now we add our Filter
            Itemlist.Filter = yourCostumFilter;

            dgRunes.ItemsSource = Itemlist;
        }

        private void butClearSorting_Click(object sender, RoutedEventArgs e)
        {
            cmbRuneGrd.SelectedIndex = -1;
            cmbRuneLoc.SelectedIndex = -1;
            cmbRuneLvl.SelectedIndex = -1;
            cmbRuneMain.SelectedIndex = -1;
            cmbRuneSet.SelectedIndex = -1;
            cmbRuneSlot.SelectedIndex = -1;
        }

        private void cmbRuneSet_DropDownClosed(object sender, EventArgs e)
        {
            DGSortRunes();
        }

        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            Monsters.Clear();
            Runes.Clear();
            RuneLocations.Clear();
            RuneLocations.Add("Iventory");
            dgMonsters.Items.Refresh();
            dgRunes.Items.Refresh();
            lblImpExpStatus.Content = "Resetted!";
        }

        private void butOptReset_Click(object sender, RoutedEventArgs e)
        {
            cmbOptMonsters.SelectedIndex = -1;
            cmbOptSet1.SelectedIndex = -1;
            cmbOptSet2.SelectedIndex = -1;
            cmbOptSet3.SelectedIndex = -1;
            cmbOptSlot2.SelectedIndex = -1;
            cmbOptSlot4.SelectedIndex = -1;
            cmbOptSlot6.SelectedIndex = -1;
        }

        private void butOptimize_Click(object sender, RoutedEventArgs e)
        {
            //OptimizeList.Clear();
                        
            if (cmbOptMonsters.SelectedItem != null)
            {
                selectedRuneSets.Clear();
                
                //Makes list of selected runesets
                if (cmbOptSet1.SelectedIndex != -1)
                    selectedRuneSets.Add(cmbOptSet1.SelectedValue.ToString());
                if (cmbOptSet2.SelectedIndex != -1)
                    selectedRuneSets.Add(cmbOptSet2.SelectedValue.ToString());
                if (cmbOptSet3.SelectedIndex != -1)
                    selectedRuneSets.Add(cmbOptSet3.SelectedValue.ToString());

                //Makes list of selected runeslots
                if (cmbOptSlot2.SelectedIndex != -1)
                    selectedRuneSlot2 = cmbOptSlot2.SelectedValue.ToString();
                if (cmbOptSlot4.SelectedIndex != -1)
                    selectedRuneSlot4 = cmbOptSlot4.SelectedValue.ToString();
                if (cmbOptSlot6.SelectedIndex != -1)
                    selectedRuneSlot6 = cmbOptSlot6.SelectedValue.ToString();

                var selMob = from y in Monsters where y.Monster_Name == cmbOptMonsters.SelectedValue.ToString() select y;
                ref2Monster = selMob.First();
                
                worker.RunWorkerAsync();
            }
        }

        //Summs the stat, ex sums the total attack of a mob
        public Optimized SumStat (Monster tempMon, Rune r1, Rune r2, Rune r3, Rune r4, Rune r5, Rune r6, double i)
        {
            List<Rune> tempList = new List<Rune>() { r1, r2, r3, r4, r5, r6 };
            
            IEnumerable<Rune> tempRunes = tempList as IEnumerable<Rune>;

            UpdateMonsterStats(tempMon, tempRunes);

            double atk = tempMon.Monster_Set_Atk + tempMon.Monster_Base_Atk;
            double def = tempMon.Monster_Set_Def + tempMon.Monster_Base_Def;
            double hp = tempMon.Monster_Set_HP + tempMon.Monster_Base_HP;
            double spd = tempMon.Monster_Set_Spd + tempMon.Monster_Base_Spd;
            int crate = tempMon.Monster_Set_CritRate + tempMon.Monster_Base_CritRate;
            int cdmg = tempMon.Monster_Set_CritDmg + tempMon.Monster_Base_CritDmg;
            int acc = tempMon.Monster_Set_Acc + tempMon.Monster_Base_Acc;
            int res = tempMon.Monster_Set_Res + tempMon.Monster_Base_Res;
            /*
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.xml") == true)
            {
                XElement root = new XElement("Optimized");
                root.Add(new XAttribute("id", i));
                root.Add(new XElement("runeids", String.Join(",", r1.Rune_ID, r2.Rune_ID, r3.Rune_ID, r4.Rune_ID, r5.Rune_ID, r6.Rune_ID)));
                root.Add(new XElement("atk", atk));
                root.Add(new XElement("def", def));
                root.Add(new XElement("hp", hp));
                root.Add(new XElement("crate", crate));
                root.Add(new XElement("cdmg", cdmg));
                root.Add(new XElement("acc", acc));
                root.Add(new XElement("spd", spd));
                root.Add(new XElement("res", res));
                xmlDocument.Element("Optimizer").Add(root);
            }*/
            /*
            else
            {
                XDocument xmlDocument = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),  
                    new XElement("Optimizer", 
                        new XElement("Optimized", new XAttribute("id", 1), 
                            new XElement("runeids", String.Join(",", r1.Rune_ID, r2.Rune_ID, r3.Rune_ID, r4.Rune_ID, r5.Rune_ID, r6.Rune_ID)), 
                            new XElement("atk", atk),
                            new XElement("def", def),
                            new XElement("hp", hp),
                            new XElement("crate", crate),
                            new XElement("cdmg", cdmg),
                            new XElement("acc", acc),
                            new XElement("spd", spd),
                            new XElement("res", res))));
                xmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.xml");
            }*/


            Optimized optNew = new Optimized(i, String.Join(",", r1.Rune_ID, r2.Rune_ID, r3.Rune_ID, r4.Rune_ID, r5.Rune_ID, r6.Rune_ID), atk,def,hp,crate,cdmg,acc,spd,res);
            return (optNew);
            /*
            
            DataSet dsOptExport = new DataSet();
            dsOptExport.Tables.Add(tblOptimizer);

            dsOptExport.WriteXml(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\Optimizer.xml");
            */

            
            //Optimized opt = new Optimized(OptimizeList.Count + 1, String.Join(",", r1.Rune_ID, r2.Rune_ID, r3.Rune_ID, r4.Rune_ID, r5.Rune_ID, r6.Rune_ID),atk,def,hp,crate,cdmg,acc,spd,res);
            //OptimizeList.Add(opt);
        }

        private void butOptStop_Click(object sender, RoutedEventArgs e)
        {
            if (worker.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                worker.CancelAsync();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();
        }
    }   
}
