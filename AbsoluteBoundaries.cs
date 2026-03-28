using System;
using System.Data;

namespace ImpostersOrdeal
{
    public static class AbsoluteBoundaries
    {
        private static DataTable table = new DataTable();

        static AbsoluteBoundaries()
        {
            table = new DataTable();

            DataColumn[] columns = {
                new DataColumn("Value", typeof(string)),
                new DataColumn("Minimum", typeof(int)),
                new DataColumn("Maximum", typeof(int)),
                new DataColumn("Increment", typeof(int)),
            };

            table.Columns.AddRange(columns);

            columns[0].ReadOnly = true;

            table.Rows.Add(new object[] { "Level", 1, 100, 1 });
            table.Rows.Add(new object[] { "Base Stat", 1, 255, 1 });
            table.Rows.Add(new object[] { "Catch Rate", 1, 255, 1 });
            table.Rows.Add(new object[] { "EV Yield", 0, 3, 1 });
            table.Rows.Add(new object[] { "EV Yield Total", 1, 3, 1 });
            table.Rows.Add(new object[] { "Initial Friendship", 0, 250, 10 });
            table.Rows.Add(new object[] { "EXP Yield", 1, 65535, 1 });
            table.Rows.Add(new object[] { "Level Up Move Count", 1, 255, 1 });
            table.Rows.Add(new object[] { "Egg Move Count", 0, 255, 1 });
            table.Rows.Add(new object[] { "Power", 5, 255, 5 });
            table.Rows.Add(new object[] { "Accuracy", 5, 100, 5 });
            table.Rows.Add(new object[] { "PP", 5, 40, 5 });
            table.Rows.Add(new object[] { "Trainer Pokémon Count", 1, 6, 1 });
            table.Rows.Add(new object[] { "IV", 0, 31, 1 });
            table.Rows.Add(new object[] { "EV", 0, 255, 1 });
            table.Rows.Add(new object[] { "EV Total", 0, 510, 1 });
            table.Rows.Add(new object[] { "Price", 10, 999990, 10 });
        }

        public static DataTable GetTable()
        {
            return table;
        }

        public static DataRow GetBoundaries(Boundary a)
        {
            if (a == Boundary.None)
                return null;

            return table.Rows[(int)a];
        }

        public static bool IsWithin(Boundary a, int x)
        {
            if (a == Boundary.None)
                return true;

            DataRow b = GetBoundaries(a);
            return x >= (int)b[1] && x <= (int)b[2];
        }

        public static int Conform(Boundary a, int x)
        {
            if (a == Boundary.None)
                return x;

            DataRow b = GetBoundaries(a);
            x = (int)(Math.Round((double)x / (int)b[3]) * (int)b[3]);
            return Math.Clamp(x, (int)b[1], (int)b[2]);
        }

        public enum Boundary
        {
            Level,
            BaseStat,
            CatchRate,
            EvYield,
            EvYieldTotal,
            InitialFriendship,
            ExpYield,
            LevelUpMoveCount,
            EggMoveCount,
            Power,
            Accuracy,
            Pp,
            TrainerPokemonCount,
            Iv,
            Ev,
            EvTotal,
            Price,
            None = -1
        }
    }
}
