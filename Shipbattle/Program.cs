using System;

namespace Shipbattle
{
    public class Shipbattle
    {
        private const int
            FieldSize = 10,
            //Метка проверки поля с кораблём. Нужна, чтобы не считать один корабль дважды
            Visited = 2;

        private static void Main(string[] args)
        {
        }

        public static bool IsCorrectBattleField(int[,] battleField)
        {
            if (battleField.GetLength(0) != FieldSize || battleField.GetLength(1) != FieldSize)
                throw new ArgumentException("Поле недопустимого размера.");
            var counts = new[] {0, 0, 0, 0};

            for (var i = 0; i < FieldSize; i++)
            for (var j = 0; j < FieldSize; j++)
                if (battleField[i, j] == 1)
                {
                    var ship = WhatIsShip(battleField, i, j);
                    if (ship == Ship.Error)
                    {
                        RevertBattleField(battleField);
                        return false;
                    }

                    counts[(int) ship]++;
                }
            //Удаляем метки
            RevertBattleField(battleField);
            //Проверка количества
            for (var i = 0; i < 4; i++)
                if (counts[i] != 4 - i)
                    return false;
            return true;
        }

        private static Ship WhatIsShip(int[,] battleShip, int x, int y)
        {
            if (x == 10 || y == 10 || battleShip[x, y] == 0)
                return Ship.Empty;
            //Проверка диагоналей
            if (x != 9 && y != 9 && battleShip[x + 1, y + 1] != 0 ||
                x != 9 && y != 0 && battleShip[x + 1, y - 1] != 0 ||
                x != 0 && y != 9 && battleShip[x - 1, y + 1] != 0 ||
                x != 0 && y != 0 && battleShip[x - 1, y - 1] != 0)
                return Ship.Error;
            //Чтобы не считать одно поле дважды
            battleShip[x, y] = Visited;
            //Можно смотреть только вправо и вниз, так как слева и сверху всё уже посчитано
            var gShip = WhatIsShip(battleShip, x, y + 1);
            var vShip = WhatIsShip(battleShip, x + 1, y);
            //Если хотя бы один вызов нашёл ошибку, то пробрасываем её наверх. Если нашли и горизонтальный, и вертикальный корабль, то текущий корабль не является линейным
            if (gShip == Ship.Error || vShip == Ship.Error || gShip != Ship.Empty && vShip != Ship.Empty)
                return Ship.Error;
            var maxShip = vShip == Ship.Empty ? gShip : vShip;
            return maxShip + 1;
        }

        /// <summary>
        /// Возвращает поле к первоначальному виду
        /// </summary>
        /// <param name="battleField"></param>
        private static void RevertBattleField(int[,] battleField)
        {
            for (var i = 0; i < FieldSize; i++)
            for (var j = 0; j < FieldSize; j++)
                if (battleField[i, j] == Visited)
                    battleField[i, j] = 1;
        }

        private enum Ship
        {
            Submarine = 0,
            Destroer = 1,
            Cruiser = 2,
            Battleship = 3,
            Error = 4,
            Empty = -1
        }
    }
}