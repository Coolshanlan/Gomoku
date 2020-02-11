using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 五子棋
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Gomoku[,] Chess_Now = new Gomoku[15,15];
        double[] Initialize_weight = new double[]  {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
            0,1,2,2,2,2,2,2,2,2,2,2,2,1,0,
            0,1,2,3,3,3,3,3,3,3,3,3,2,1,0,
            0,1,2,3,4,4,4,4,4,4,4,3,2,1,0,
            0,1,2,3,4,5,5,5,5,5,4,3,2,1,0,
            0,1,2,3,4,5,6,6,6,5,4,3,2,1,0,
            0,1,2,3,4,5,6,7,6,5,4,3,2,1,0,
            0,1,2,3,4,5,6,6,6,5,4,3,2,1,0,
            0,1,2,3,4,5,5,5,5,5,4,3,2,1,0,
            0,1,2,3,4,4,4,4,4,4,4,3,2,1,0,
            0,1,2,3,3,3,3,3,3,3,3,3,2,1,0,
            0,1,2,2,2,2,2,2,2,2,2,2,2,1,0,
            0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        Button[,] bu = new Button[15, 15];
        Button[,] bu2 = new Button[15, 15];
        public void bu_add()
        {
            for(int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    bu[i, j] = new Button();
                    bu[i, j].FlatStyle = FlatStyle.Flat;
                    bu[i, j].BackColor =  Color.FromArgb(255,220,160,101);
                    bu[i, j].Font = new Font("新細明體", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                    bu[i, j].Location = new Point(10+ 32 * i, 15 + 32 * j);
                    bu[i, j].Size = new Size(32, 32);
                    bu[i, j].Text = "";
                    bu[i, j].Name = (i * 15 + j).ToString();
                    bu[i, j].Click += new EventHandler(bu_Click);
                    bu[i, j].Name = i.ToString() + "," + j.ToString();
                    Chess_Now[i, j] = new Gomoku();
                    Chess_Now[i, j].chess_color = -1;
                    groupBox1.Controls.Add(bu[i, j]);
                    bu2[i, j] = new Button();
                    bu2[i, j].FlatStyle = FlatStyle.Flat;
                    bu2[i, j].BackColor = Color.FromArgb(255, 220, 160, 101);
                    bu2[i, j].Font = new Font("新細明體", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
                    bu2[i, j].Location = new Point(10 + 32 * i, 15 + 32 * j);
                    bu2[i, j].Size = new Size(32, 32);
                    bu2[i, j].Text = "";
                    bu2[i, j].Name = (i * 15 + j).ToString();
                    bu2[i, j].Click += new EventHandler(bu_Click);
                    bu2[i, j].Name = i.ToString() + "," + j.ToString();
                    groupBox2.Controls.Add(bu2[i, j]);
                }
            }
        }
        int user_color = -1;//0黑1白
        int turn = 1;//0黑1白
        bool game_over = false;
        private void bu_Click(object sender, EventArgs e)
        {
            Button down_bu = (Button)sender;
            string[] ss = down_bu.Name.Split(',');
            Point down = new Point(Convert.ToInt32(ss[0]), Convert.ToInt32(ss[1]));
            bu2[down.X, down.Y].BackColor = Color.LightPink;
            if (user_color == -1) return;
            if (Chess_Now[down.X, down.Y].chess_color != -1) return;


            if (game_over) return;


            turn++;

            Chess_Now[down.X, down.Y].chess_color = turn % 2;
            down_bu.Text = "●";
            if (turn%2 == 0)
            {
                down_bu.ForeColor = Color.Black;
                down_bu.FlatAppearance.BorderColor = Color.Black;
            }
            else
            {
                down_bu.ForeColor = Color.White;
                down_bu.FlatAppearance.BorderColor = Color.Black;
            }

            int win = Check_Win(Chess_Now);

            if (win != -1)
            {
                game_over = true;
                MessageBox.Show(win == 0 ? "黑子勝" : "白子勝");
            }
            if (Mode != "Two Player")
                if (turn % 2 == user_color && win == -1)
                {
                    tree_value down_point = Computer(Chess_Now, (user_color + 1) % 2);
                    bu[down_point.point.X, down_point.point.Y].PerformClick();
                }
        }
        Gomoku[,] playbook_aa = new Gomoku[15, 15];
        public tree_value Computer(Gomoku[,] playbook,int computer_color)
        {
            //return Computer_Tree_Search(playbook, computer_color, 6, 1, 4);
            //return Computer_Tree_Search(playbook,computer_color,4,1,6);
            //weight_update(playbook, computer_color);
            //return Select_Max_Point(playbook);

            if(Mode == "AI_Integral")
            {
                weight_update(playbook, computer_color);
                List<Point> p = new List<Point>();
                p = selectsearch2(playbook);
                Gomoku[,] orgin = new Gomoku[15,15];
                for(int i = 0; i < 15; i++)
                {
                    for (int j= 0; j < 15; j++)
                    {
                        orgin[i, j] = playbook[i, j].Clone();
                    }
                }
                for (int i = 0; i < p.Count; i++)
                {
                       playbook[p[i].X, p[i].Y].chess_color = ((user_color +1)%2);
                        bool ans = endwin(user_color , playbook, 4);
                    if (ans == true) {
                        tree_value t = new tree_value();
                        t.point = new Point(p[i].X,p[i].Y);
                        for (int a = 0; a < 15; a++)
                        {
                            for (int s = 0; s < 15; s++)
                            {
                                playbook[a, s] = orgin[a, s].Clone();
                            }
                        }
                        return t;
                       }
                    for (int a = 0; a < 15; a++)
                    {
                        for (int s = 0; s < 15; s++)
                        {
                            playbook[a, s] = orgin[a, s].Clone();
                        }
                    }
                }

                return Select_Max_Point(playbook);
            }
            if (Mode == "AI_Tree2(test)")return Computer_Tree_Search(playbook, computer_color, 2, 1, 30);
            if (Mode == "AI_Tree3(test)")return Computer_Tree_Search(playbook, computer_color, 3, 1, 20);
            if (Mode == "AI_Tree4(test)")return Computer_Tree_Search(playbook, computer_color, 4, 1, 7);
            if (Mode == "AI_Tree5(test)")return Computer_Tree_Search(playbook, computer_color, 5, 1, 5);
            if (Mode == "AI_Tree6(test)")return Computer_Tree_Search(playbook, computer_color, 6, 1, 4);


            return null;
        }
        public tree_value Computer_Tree_Search(Gomoku[,] playbook, int tree_color ,int tree_times,int now_times,int select_point)
        {
            
            Gomoku[,] tree_playbook = new Gomoku[15,15];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    tree_playbook[i, j] = playbook[i, j].Clone();
                }
            }

            List<tree_value> candown = new List<tree_value>();
            weight_update(tree_playbook,tree_color);

            for(int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (tree_playbook[i, j].chess_color == -1 && tree_playbook[i, j].weight - integralchess[i * 15 + j] !=0 ) candown.Add(new tree_value() { point = new Point(i, j), weight = tree_playbook[i, j].weight });
                }
            }

            candown.Sort((x, y) => -x.weight.CompareTo(y.weight));

            if (now_times < tree_times)
            {
                for (int i=0; i < ((candown.Count>select_point)? select_point : candown.Count); i++)
                {
                    tree_playbook[candown[i].point.X, candown[i].point.Y].chess_color = tree_color % 2;
                    tree_value t_v = Computer_Tree_Search(tree_playbook, tree_color + 1, tree_times, now_times + 1,select_point);
                    tree_playbook[candown[i].point.X, candown[i].point.Y].weight += t_v.weight;
                    tree_playbook[candown[i].point.X, candown[i].point.Y].chess_color = -1;
                }
            }
            tree_value t_v_a = Select_Max_Point(tree_playbook);
            if(now_times != 1)
            t_v_a.weight *= -0.3/*((now_times % 2) == 1 ? 1 : -1)*/;
            if(now_times == 1)
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    if (tree_playbook[i, j].chess_color == -1 || (i == t_v_a.point.X && j == t_v_a.point.Y))
                        {
                            bu2[i, j].Text = tree_playbook[i, j].weight.ToString();
                          
                        }
                        else
                        {
                            bu2[i, j].Text = "";
                        }
                        bu2[i, j].BackColor = Color.FromArgb(255, 220, 160, 101);
                    }
            return t_v_a;
        }

        public tree_value Select_Max_Point(Gomoku[,] playbook)
        {
            double weight = 0;

            tree_value Down = new tree_value();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (playbook[i, j].chess_color == -1 && playbook[i, j].weight > weight )
                    {
                        weight = playbook[i, j].weight;
                        Down = new tree_value() { point = new Point(i, j), weight = weight };
                    }
                }
            }

            return Down;
        }

        public void weight_update(Gomoku[,] playbook , int computer_color)
        {
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    int must_down_computer = 0;
                    int must_down_people = 0;
                    if (playbook[i, j].chess_color != -1) continue;
                    playbook[i, j].weight = integralchess[i * 15 + j];
                    for (int w = -1; w < 3; w++)
                    {
                        search_value s_v = new search_value();
                        int x_w = w == -1 ? -1 : w - 1;
                        int y_w = w == -1 ? 0 : 1;
                        int x = i + x_w;
                        int y = j + y_w;
                        double weight = 0;


                        if (x >= 0 && y >= 0 && x < 15 && y < 15)
                        {
                            s_v.color = playbook[x, y].chess_color;
                            search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                        }


                        x_w = x_w * -1;
                        y_w = y_w * -1;
                        x = i + x_w;
                        y = j + y_w;

                      

                        if (x >= 0 && y >= 0 && x < 15 && y < 15)
                        {
                            if (playbook[x, y].chess_color != s_v.color && playbook[x, y].chess_color != -1 && s_v.color != -1) s_v.dead = "d";
                            if (playbook[x, y].chess_color == s_v.color)
                            {
                                search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                            }
                            else
                            {
                                if(side(playbook,i,j,x_w , y_w ,s_v))
                                   weight += s_v.color == computer_color ? integral_computer(s_v) : integral_people(s_v);
                                if (s_v.count == 2 && s_v.dead == null && s_v.color == computer_color) must_down_computer++;
                                if (s_v.count == 2 && s_v.dead == null && s_v.color != computer_color)
                                {
                                    must_down_people++;
                                }
                                if (s_v.count == 3 && s_v.color == computer_color) must_down_computer++;
                                if (s_v.count == 3 && s_v.color != computer_color)
                                {
                                    must_down_people++;
                                }
                                s_v = new search_value();
                                s_v.color = playbook[x, y].chess_color;
                                search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                            }
                        }
                        if (side(playbook, i, j, x_w, y_w, s_v))
                            weight += s_v.color == computer_color ? integral_computer(s_v) : integral_people(s_v);
                        if (s_v.count == 2 && s_v.dead == null && s_v.color == computer_color%2) must_down_computer++;
                        if (s_v.count == 2 && s_v.dead == null && s_v.color != computer_color%2)
                        {
                            must_down_people++;
                        }
                        if (s_v.count == 3 && s_v.color == computer_color%2) must_down_computer++;
                        if (s_v.count == 3 && s_v.color != computer_color%2)
                        {
                            must_down_people++;
                        }

                        playbook[i, j].weight += weight;
                    }

                    if (must_down_people >= 2)
                    {
                        playbook[i, j].weight += 200;
                    }
                    if (must_down_computer >= 2)
                    {
                        playbook[i, j].weight += 250;
                    }

                }
        }

        public int Check_Win(Gomoku[,] playbook)
        {
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    //if (playbook[i, j].chess_color != -1) continue;
                    for (int w = -1; w < 3; w++)
                    {
                        search_value s_v = new search_value();
                        int x_w = w == -1 ? -1 : w - 1;
                        int y_w = w == -1 ? 0 : 1;
                        int x = i + x_w;
                        int y = j + y_w;


                        if (x >= 0 && y >= 0 && x < 15 && y < 15)
                        {
                            s_v.color = playbook[x, y].chess_color;
                            search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                        }

                        if (playbook[i, j].chess_color == s_v.color) s_v.count++;

                        if (s_v.count >= 5)
                        {
                            return s_v.color == 0 ? 0 : 1;
                        }

                        if (playbook[i, j].chess_color == s_v.color) s_v.count--;

                        x_w *= -1;
                        y_w *= -1;
                        x = i + x_w;
                        y = j + y_w;

                        if (x >= 0 && y >= 0 && x < 15 && y < 15)
                        {
                            if (playbook[x, y].chess_color == s_v.color && playbook[i,j].chess_color == s_v.color)
                            {
                                search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                                if (playbook[i, j].chess_color == s_v.color) s_v.count++;
                                if (s_v.count >= 5)
                                {
                                    return s_v.color == 0 ? 0 : 1;
                                }
                            }
                            else
                            {
                                s_v = new search_value();
                                s_v.color = -1;
                                s_v.color = playbook[x, y].chess_color;
                                search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                                if (playbook[i, j].chess_color == s_v.color) s_v.count++;
                                if (s_v.count >= 5)
                                {
                                    s_v = new search_value();
                                    s_v.color = playbook[x, y].chess_color;
                                    search_point(playbook, s_v, s_v.color, x, y, x_w, y_w);
                                    return s_v.color == 0 ? 0 : 1;
                                }
                            }
                        }
                    }
                }
            return -1;
        }

        public void search_point(Gomoku[,] playbook , search_value s_v , int first_color ,int x , int y , int x_w , int y_w)//給搜尋起始點
        {

            if (x > 14 || y > 14 || x < 0 || y < 0) return;
            if (playbook[x, y].chess_color == -1) return;
            if (playbook[x,y].chess_color == first_color)
            {
                s_v.count++;
                x += x_w;
                y += y_w;
                search_point(playbook, s_v, first_color, x, y, x_w, y_w);
            }
            else  s_v.dead += "d";
            return;
        }

        private int integral_people(search_value s_v)
        {
            int integralcount = 0;
            string s = s_v.dead + s_v.count;
            if (s_v.count >= 4) return 2000;
            if (s_v.dead == "dd" && s_v.count < 4)
            {
                return 0;
            }
            switch (s)
            {
                case "0":
                    integralcount = 0;
                    break;
                case "1":
                    integralcount = 13;
                    break;
                case "2":
                    integralcount = 60;
                    break;
                case "3":
                    integralcount = 290;
                    break;
                case "d1":
                    integralcount = 0;
                    break;
                case "d2":
                    integralcount = 2;
                    break;
                case "d3":
                    integralcount = 30;
                    break;
            }
            return integralcount;
        }

        private int integral_computer( search_value s_v)
        {
            int integralcount = 0;
            string s = s_v.dead + s_v.count;
            if (s_v.count >= 4) return 10500;
            if (s_v.dead == "dd" && s_v.count < 4)
            {
                return 0;
            }
            switch (s)
            {
                case "0":
                    integralcount = 0;
                    break;
                case "1":
                    integralcount = 30;
                    break;
                case "2":
                    integralcount =100;
                    break;
                case "3":
                    integralcount = 600;
                    break;
                case "d1":
                    integralcount = 0;
                    break;
                case "d2":
                    integralcount = 5;
                    break;
                case "d3":
                    integralcount = 60;
                    break;
            }
            return integralcount;
        }

        public bool side(Gomoku[,] playbook, int x, int y , int x_w , int y_w ,search_value s_v)//dd進入
        {
            int last_count = 0;
            int x_s = x;
            int y_s = y;
             x_s += x_w;
             y_s += y_w;
             while(x_s >= 0 && y_s >= 0 && x_s < 15 && y_s < 15)
             {
                 if (playbook[x_s, y_s].chess_color == -1) last_count++;
                 else if (playbook[x_s, y_s].chess_color != s_v.color) break;
                 x_s += x_w;
                 y_s += y_w;
                if (last_count > 5) return true;
            }

             x_s = x;
             y_s = y;

             x_w *= -1;
             y_w *= -1;

             x_s += x_w;
             y_s += y_w;

             while (x_s >= 0 && y_s >= 0 && x_s < 15 && y_s < 15)
             {
                 if (playbook[x_s, y_s].chess_color == -1) last_count++;
                 else if (playbook[x_s, y_s].chess_color != s_v.color) break;
                 x_s += x_w;
                 y_s += y_w;
                if (last_count > 5) return true;
            }

            if (s_v.count + last_count >= 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        List<int> integralchess = new List<int> {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
            0,1,2,2,2,2,2,2,2,2,2,2,2,1,0,
            0,1,2,3,3,3,3,3,3,3,3,3,2,1,0,
            0,1,2,3,4,4,4,4,4,4,4,3,2,1,0,
            0,1,2,3,4,5,5,5,5,5,4,3,2,1,0,
            0,1,2,3,4,5,6,6,6,5,4,3,2,1,0,
            0,1,2,3,4,5,6,7,6,5,4,3,2,1,0,
            0,1,2,3,4,5,6,6,6,5,4,3,2,1,0,
            0,1,2,3,4,5,5,5,5,5,4,3,2,1,0,
            0,1,2,3,4,4,4,4,4,4,4,3,2,1,0,
            0,1,2,3,3,3,3,3,3,3,3,3,2,1,0,
            0,1,2,2,2,2,2,2,2,2,2,2,2,1,0,
            0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

        private void Form1_Load(object sender, EventArgs e)
        {
            bu_add();
            comboBox1.SelectedIndex = 1;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    playbook_aa[i, j] = Chess_Now[i, j].Clone();
                }
            }
        }
        private bool endwin(int nowcolor, Gomoku[,] now_playbook , int deeptimes  )
        {

            int iw = Check_Win(now_playbook);
            if (iw == (user_color+1)%2)
            {
                return true;
            }
            else if (iw != -1)
            {
                return false;
            }

            if (deeptimes == 0) return false;

            weight_update(now_playbook, (nowcolor)%2);


            Gomoku[,] orgin_playbook = new Gomoku[15,15];
            for (int a = 0; a < 15; a++)
            {
                for (int s = 0; s < 15; s++)
                {
                    orgin_playbook[a, s] = now_playbook[a, s].Clone();
                }
            }

            List<Point> cansearch = selectsearch(now_playbook);
            if (cansearch.Count == 0) return false;
            for(int i = 0; i < cansearch.Count; i++)
            {
                now_playbook[cansearch[i].X, cansearch[i].Y].chess_color = (nowcolor%2);
                bool ans = endwin(nowcolor+1,now_playbook, deeptimes-1);
                if ((nowcolor % 2) == (user_color+1)%2 && ans == true)
                {
                    return true;
                }
                else if ((nowcolor % 2) ==  user_color && ans == false)
                {
                    return false;
                }
                for (int a = 0; a < 15; a++)
                {
                    for (int s = 0; s < 15; s++)
                    {
                        now_playbook[a, s] = orgin_playbook[a, s].Clone();
                    }
                }
            }
            if((nowcolor % 2) == user_color)
            return true;
            else return false;


        }

        private     List<Point > selectsearch(Gomoku[,] now_playbook)
        {
            List<Point> p = new List<Point>();

            for(int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                   if(  now_playbook[i, j].chess_color != -1 || now_playbook[i, j].weight <80)
                    {
                        continue;
                    }
                    bool have = false;
                    for(int ii = -1; ii <2; ii++)
                    {
                        for (int jj = -1; jj < 2; jj++)
                        {
                            int xx = i + ii;
                            int yy = j + jj;
                            if(xx >= 0 && xx < 15 && yy >= 0 && yy < 15)
                            {
                                if(now_playbook[xx,yy].chess_color != -1)
                                {
                                    have = true;
                                    if(p.FindAll(x=>x.X ==xx && x.Y == yy).Count  == 0)
                                    p.Add(new Point(i, j));
                                    break;
                                }
                            }
                        }
                        if (have == true) break;
                    }
                }
            }

            return p;
             
        }
        private List<Point> selectsearch2(Gomoku[,] now_playbook)
        {
            List<Point> p = new List<Point>();

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (now_playbook[i, j].chess_color != -1)
                    {
                        continue;
                    }
                    bool have = false;
                    for (int ii = -2; ii < 3; ii++)
                    {
                        for (int jj = -2; jj < 3; jj++)
                        {
                            int xx = i + ii;
                            int yy = j + jj;
                            if (xx >= 0 && xx < 15 && yy >= 0 && yy < 15)
                            {
                                if (now_playbook[xx, yy].chess_color != -1)
                                {
                                    have = true;
                                    if (p.FindAll(x => x.X == xx && x.Y == yy).Count == 0)
                                        p.Add(new Point(i, j));
                                    break;
                                }
                            }
                        }
                        if (have == true) break;
                    }
                }
            }

            return p;

        }
        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            turn = 1;
            game_over = false;
            user_color = -1;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    bu[i, j].Text = "";
                    bu[i, j].ForeColor = Color.Black;
                    Chess_Now[i, j] = new Gomoku();
                    Chess_Now[i, j].chess_color = -1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user_color = 0;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(Mode == "Two Player")
            {
                user_color = 0;
                button1.Enabled = false;
                button2.Enabled = false;
                return;
            }
            user_color = 1;
            button1.Enabled = false;
            button2.Enabled = false;
            tree_value down_point = Computer(Chess_Now, 0);
            bu[down_point.point.X, down_point.point.Y].PerformClick();
        }
        string Mode = "AI_Integral";
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            Mode = comboBox1.Text;
        }
    }
    public class Gomoku
    {
        public int chess_color { get; set; } //-1,0,1
        public double weight { get; set; }
        public Gomoku Clone()
        {
            Gomoku gm = new Gomoku();
            gm.chess_color = chess_color;
            gm.weight = weight;
            return gm;
        }
    }
    public class search_value
    {
        public int count { get; set; }
        public string dead { get; set; }
        public int color { get; set; }
    }
    public class tree_value
    {
        public double weight { get; set; }
        public Point point { get; set; }
    }
}
