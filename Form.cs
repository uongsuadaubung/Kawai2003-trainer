namespace Kawai2003_trainer
{
    public partial class Form : System.Windows.Forms.Form
    {
        private readonly MyMemory _memory;
        private int _baseAddr;

        private const int Rows = 11;
        private const int Cols = 18;
        private readonly Item[,] _map = new Item[Rows, Cols];
        private const int WmLbuttondown = 0x0201;
        private const int WmRbuttondown = 0x0204;

        private readonly MyPoint[] _directions =
        {
            new() { Row = -1, Col = 0 }, // di chuyển lên
            new() { Row = 1, Col = 0 }, // di chuyển xuống
            new() { Row = 0, Col = -1 }, // di chuyển trái
            new() { Row = 0, Col = 1 }, // di chuyển phải
        };
        public Form()
        {
            InitializeComponent();
            _memory = new MyMemory("Kawai2003");
            if (!_memory.IsOk())
            {
                MessageBox.Show("Bật game lên trước");
                this.Close();
            }
            else Init();
        }
        private void Init()
        {
            _baseAddr = _memory.GetBaseAddress();
        }
        private void UpdateMap()
        {
            int[] idFirstValueOffset = { 0x000E0068 + _baseAddr, 0x76 };
            int addrFirstId = _memory.GetAddressFromPointer(idFirstValueOffset);

            int addrFirstDisplay = addrFirstId - 0x4;

            int currentDisplay = addrFirstDisplay;
            int currentId = addrFirstId;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (i == 0 || j == 0 || i == Rows - 1 || j == Cols - 1)
                    {
                        _map[i, j] = new Item { Id = -1, Show = 0 };
                    }
                    else
                    {
                        int valueId = _memory.ReadUShort(currentId);
                        int valueDisplay = _memory.ReadUShort(currentDisplay);
                        _map[i, j] = new Item { Id = valueId, Show = valueDisplay };
                        //memory.WriteNumber(currentId, 25, 2);
                        currentDisplay += 0x6;
                        currentId += 0x6;
                    }
                }

                if (i is 0 or Rows - 1)
                {
                    continue;
                }

                currentDisplay = addrFirstDisplay + (0x6c * i);
                currentId = addrFirstId + (0x6c * i);
            }
        }

        private bool IsValidMove(int newRow, int newCol, MyPoint end)
        {
            if (newRow < 0 ||
                newRow >= Rows ||
                newCol < 0 ||
                newCol >= Cols)
            {
                return false;
            }

            if (_map[newRow, newCol].Id == -1 || _map[newRow, newCol].Show == 0)
            {
                return true;
            }

            return _map[newRow, newCol].Id != -1 && _map[newRow, newCol].Show > 0 && newCol == end.Col &&
                   newRow == end.Row;
        }
        private bool IsValidConnectDfs(MyPoint start, MyPoint end)
        {
            bool[,] visited = new bool[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    visited[i, j] = false;
                }
            }

            visited[start.Row, start.Col] = true;

            return Dfs(start, 0, null);

            bool Dfs(MyPoint position, int redirect, MyPoint? lastDirection)
            {
                if (redirect > 3)
                {
                    return false;
                }

                if (position.Col == end.Col && position.Row == end.Row)
                {
                    return true;
                }

                foreach (MyPoint direction in _directions)
                {
                    int newRow = position.Row + direction.Row;
                    int newCol = position.Col + direction.Col;
                    if (!IsValidMove(newRow, newCol, end) || visited[newRow, newCol]) continue;
                    visited[newRow, newCol] = true;

                    int newRedirect = redirect;
                    if (
                        lastDirection == null ||
                        lastDirection.Row != direction.Row ||
                        lastDirection.Col != direction.Col
                    )
                    {
                        newRedirect++; // Nếu có chuyển hướng thì tăng biến redirect
                    }

                    if (Dfs(new MyPoint { Row = newRow, Col = newCol }, newRedirect, direction))
                    {
                        return true;
                    }

                    visited[newRow, newCol] = false;
                }

                return false;
            }
        }

        private MyPoint[]? FindAPair()
        {
            UpdateMap();
            for (int i = 1; i <= Rows - 2; i++)
            {
                for (int j = 1; j <= Cols - 2; j++)
                {
                    if (_map[i, j].Show == 0)
                    {
                        continue;
                    }

                    MyPoint poin1 = new MyPoint { Row = i, Col = j };
                    for (int k = 1; k <= Rows - 2; k++)
                    {
                        for (int l = 1; l <= Cols - 2; l++)
                        {
                            if (i == k && j == l)
                            {
                                continue;
                            }

                            if (_map[k, l].Show == 0)
                            {
                                continue;
                            }

                            if (_map[i, j].Id != _map[k, l].Id)
                            {
                                continue;
                            }

                            MyPoint poin2 = new MyPoint { Row = k, Col = l };
                            //if (i == 2 && j == 11 && k == 4)
                            //{
                            //    Console.WriteLine("for debug");
                            //}
                            if (!IsValidConnectDfs(poin1, poin2)) continue;
                            MyPoint[] myPoints = { poin1, poin2 };
                            return myPoints;
                        }
                    }
                }
            }

            return null;
        }

        private void AutoMatch()
        {
            if (cbAuto.Checked)
            {
                btnMatchOne.Enabled = false;
            }
            MyPoint[]? mypoints;
            do
            {
                mypoints = FindAPair();
                if (mypoints is not { Length: > 0 }) continue;
                _memory.ClickToCell(mypoints[0].Row, mypoints[0].Col, WmRbuttondown);
                _memory.ClickToCell(mypoints[0].Row, mypoints[0].Col, WmLbuttondown);
                Thread.Sleep(300);
                _memory.ClickToCell(mypoints[1].Row, mypoints[1].Col, WmLbuttondown);
                Thread.Sleep(300);
            } while (mypoints is { Length: > 0 } && cbAuto.Checked);

            btnMatchOne.Enabled = true;
        }

        

        private void btnMatchOne_Click(object sender, EventArgs e)
        {
            Thread thread = new(AutoMatch);
            thread.Start();
            
        }

        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            btnMatchOne.Text = cbAuto.Checked ? "Auto" : "Match one pair";
            
        }
    }
}