using System;
using TitTacToeGame;
using static TitTacToeGame.Game;

namespace TicTacToeConsoleTest

{
    internal class RandomBrain: IPlayer
    {
        static Random random = new Random();
        public RandomBrain()
        {

        }

        public void Move(Game game)
        {
            MoveDTO move;
            do
            {
                move = GenerateMove();
            }
            while (!game.CanMakeMove(move));

            game.Move(move);
        }

        private MoveDTO GenerateMove()
        {
            MoveDTO dto = new MoveDTO();
            int ubound = 3;
            dto.X = random.Next(ubound);
            dto.Y = random.Next(ubound);
            return dto;
        }
    }
}