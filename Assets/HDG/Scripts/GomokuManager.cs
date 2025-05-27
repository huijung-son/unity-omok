using System.Collections.Generic;
using UnityEngine;

public class GomokuManager : MonoBehaviour
{
    // 색깔 enum
    public enum GomokuColor
    {
        None,
        Black,
        White
    }

    // 한 칸의 상태 (흑돌, 백돌, 빔)
    // 기본적으로 isEmpty = true
    public class GomokuSquare
    {
        private GomokuColor color = GomokuColor.None;

        public GomokuColor Color { get => color; set => color = value; }
        public bool IsBlack { get { return color == GomokuColor.Black; } }
        public bool IsWhite { get { return color == GomokuColor.White; } }
        public bool IsEmpty { get { return color == GomokuColor.None; } }
    }

    // 돌 한 개의 좌표값과 색 저장
    public class GomokuStone
    {
        private int xPos = 0;
        private int yPos = 0;
        private GomokuColor color = GomokuColor.None;

        public int XPos { get; set; }
        public int YPos { get; set; }
        public GomokuColor Color { get => color; set => color = value; }
        public bool IsBlack { get { return color == GomokuColor.Black; } }
        public bool IsWhite { get { return color == GomokuColor.White; } }
    }

    //public List<GomokuStone> stoneList = new List<GomokuStone>();

    public class GomokuPlayer
    {
        // 플레이어가 흑돌인지, 백돌인지 저장
        private GomokuColor color = GomokuColor.None;
        private bool isWin = false;

        public GomokuColor Color { get; set; }
        public bool IsBlack { get { return color == GomokuColor.Black; } }
        public bool IsWhite { get { return color == GomokuColor.White; } }
        public bool IsWin { get; set; }
    }

    public class GomokuForbidden
    {
        // 금수 체크 (33, 44, 장목)
        private bool isDoubleThree = false;
        private bool isDoubleFour = false;
        private bool isOverline = false;

        public bool IsDoubleThree { get; set; }
        public bool IsDoubleFour { get; set; }
        public bool IsOverline { get; set; }
    }

    public class GomokuConnectState
    {
        // 이어진 돌 상태 (열린 3-4, 닫힌 3-4, 띈 4)
        private bool isOpenThree = false;
        private bool isClosedThree = false;
        private bool isOpenFour = false;
        private bool isClosedFour = false;
        private bool isBrokenFour = false;
        // 이어진 돌 상태를 기반으로 금수를 체크해서 bool 값 저장

        public bool IsOpenThree { get; set; }
        public bool IsClosedThree { get; set; }
        public bool IsOpenFour { get; set; }
        public bool IsClosedFour { get; set; }
        public bool IsBrokenFour { get; set; }
    }

    // 연속 3개 그룹
    //private GomokuStone[] ThreeStoneGroup = new GomokuStone[3];

    // 연속 4개 그룹
    //private GomokuStone[] FourStoneGroup = new GomokuStone[4];

    // 플레아어
    //private GomokuPlayer player = new GomokuPlayer();

    // 보드 사이즈
    public GomokuStone[,] board = new GomokuStone[15, 15];
    //public List<List<GomokuStone>> board = new List<List<GomokuStone>>();
    // 0  A B C D E F G H I J K L M N O
    // 1  · · · · · · · · · · · · · · ·
    // 2  · · · · · · · · · · · · · · ·
    // 3  · · · · · · · · · · · · · · ·
    // 4  · · · · · · · · · · · · · · ·
    // 5  · · · · · · · · · · · · · · ·
    // 6  · · · · · · · · · · · · · · ·
    // 7  · · · · · · · · · · · · · · ·
    // 8  · · · · · · · · · · · · · · ·
    // 9  · · · · · · · · · · · · · · ·
    //10  · · · · · · · · · · · · · · ·
    //11  · · · · · · · · · · · · · · ·
    //12  · · · · · · · · · · · · · · ·
    //13  · · · · · · · · · · · · · · ·
    //14  · · · · · · · · · · · · · · ·
    //15  · · · · · · · · · · · · · · ·
    // [0,0] - [14,14]

    // 5개가 이어지게 놓아졌는지 검사해서 승리 체크
    public bool CheckWin(GomokuStone _currentStone)
    {
        if (

            // 흑돌 플레이어 승리 조건 (6목 이상 시 검사해야함)
            // 돌이 놓인 위치가 테두리인 경우 인덱스가 초과하는 걸 조심해야 함
            //player.Color == GomokuColor.Black &&
            // 가로
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            // !board[_currentStone.XPos - 1, _currentStone.YPos].IsBlack && !board[_currentStone.XPos + 5, _currentStone.YPos].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 4, _currentStone.YPos].IsBlack) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            //!board[_currentStone.XPos - 2, _currentStone.YPos].IsBlack && !board[_currentStone.XPos + 4, _currentStone.YPos].IsBlack &&
            board[_currentStone.XPos - 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos].IsBlack) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            //!board[_currentStone.XPos - 3, _currentStone.YPos].IsBlack && !board[_currentStone.XPos + 3, _currentStone.YPos].IsBlack &&
            board[_currentStone.XPos - 2, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos].IsBlack) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            //!board[_currentStone.XPos - 4, _currentStone.YPos].IsBlack && !board[_currentStone.XPos + 2, _currentStone.YPos].IsBlack &&
            board[_currentStone.XPos - 3, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos].IsBlack) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            //!board[_currentStone.XPos - 5, _currentStone.YPos].IsBlack && !board[_currentStone.XPos + 1, _currentStone.YPos].IsBlack &&
            board[_currentStone.XPos - 4, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 3, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack) ||

            // 세로Í
            (_currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            //!board[_currentStone.XPos, _currentStone.YPos - 1].IsBlack && !board[_currentStone.XPos, _currentStone.YPos + 5].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 3].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 4].IsBlack) ||

            (_currentStone.YPos >= 1 && _currentStone.YPos < 12 &&
            //!board[_currentStone.XPos, _currentStone.YPos - 2].IsBlack && !board[_currentStone.XPos, _currentStone.YPos + 4].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 3].IsBlack) ||

            (_currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            //!board[_currentStone.XPos, _currentStone.YPos - 3].IsBlack && !board[_currentStone.XPos, _currentStone.YPos + 3].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 2].IsBlack) ||

            (_currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            //!board[_currentStone.XPos, _currentStone.YPos - 4].IsBlack && !board[_currentStone.XPos, _currentStone.YPos + 2].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos - 3].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos, _currentStone.YPos + 1].IsBlack) ||

            (_currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            //!board[_currentStone.XPos, _currentStone.YPos - 5].IsBlack && !board[_currentStone.XPos, _currentStone.YPos + 1].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos - 4].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 3].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack) ||

            // 좌상단 대각선
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            _currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            //!board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsBlack && !board[_currentStone.XPos + 5, _currentStone.YPos + 5].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos + 3].IsBlack && board[_currentStone.XPos + 4, _currentStone.YPos + 4].IsBlack) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            _currentStone.YPos >= 1 && _currentStone.YPos < 12 &&
            //!board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsBlack && !board[_currentStone.XPos + 4, _currentStone.YPos + 4].IsBlack &&
            board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos + 3].IsBlack) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            _currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            //!board[_currentStone.XPos - 3, _currentStone.YPos - 3].IsBlack && !board[_currentStone.XPos + 3, _currentStone.YPos + 3].IsBlack &&
            board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsBlack) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            _currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            //!board[_currentStone.XPos - 4, _currentStone.YPos - 4].IsBlack && !board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsBlack &&
            board[_currentStone.XPos - 3, _currentStone.YPos - 3].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsBlack) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            _currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            //!board[_currentStone.XPos - 5, _currentStone.YPos - 5].IsBlack && !board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsBlack &&
            board[_currentStone.XPos - 4, _currentStone.YPos - 4].IsBlack && board[_currentStone.XPos - 3, _currentStone.YPos - 3].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack) ||

            // 우상단 대각선
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            _currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            //!board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsBlack && !board[_currentStone.XPos + 5, _currentStone.YPos - 5].IsBlack &&
            board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos - 3].IsBlack && board[_currentStone.XPos + 4, _currentStone.YPos - 4].IsBlack) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            _currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            //!board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsBlack && !board[_currentStone.XPos + 4, _currentStone.YPos - 4].IsBlack &&
            board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsBlack && board[_currentStone.XPos + 3, _currentStone.YPos - 3].IsBlack) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            _currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            //!board[_currentStone.XPos - 3, _currentStone.YPos + 3].IsBlack && !board[_currentStone.XPos + 3, _currentStone.YPos - 3].IsBlack &&
            board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsBlack && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsBlack) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            _currentStone.YPos >= 1 && _currentStone.YPos < 11 &&
            //!board[_currentStone.XPos - 4, _currentStone.YPos + 4].IsBlack && !board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsBlack &&
            board[_currentStone.XPos - 3, _currentStone.YPos + 3].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsBlack) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            _currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            //!board[_currentStone.XPos - 5, _currentStone.YPos + 5].IsBlack && !board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsBlack &&
            board[_currentStone.XPos - 4, _currentStone.YPos + 4].IsBlack && board[_currentStone.XPos - 3, _currentStone.YPos + 3].IsBlack && board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsBlack && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsBlack && board[_currentStone.XPos, _currentStone.YPos].IsBlack)
            )
        {
            //player.IsWin = true;
            return true;
        }

        else if (
            // 백돌 플레이어 승리 조건
            // 백돌은 장목시에도 승리
            //player.Color == GomokuColor.White &&

            // 가로
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 4, _currentStone.YPos].IsWhite) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            board[_currentStone.XPos - 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos].IsWhite) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            board[_currentStone.XPos - 2, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos].IsWhite) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            board[_currentStone.XPos - 3, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos].IsWhite) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            board[_currentStone.XPos - 4, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 3, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite) ||

            // 세로
            (_currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 3].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 4].IsWhite) ||

            (_currentStone.YPos >= 1 && _currentStone.YPos < 12 &&
            board[_currentStone.XPos, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 3].IsWhite) ||

            (_currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            board[_currentStone.XPos, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 2].IsWhite) ||

            (_currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            board[_currentStone.XPos, _currentStone.YPos - 3].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos, _currentStone.YPos + 1].IsWhite) ||

            (_currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            board[_currentStone.XPos, _currentStone.YPos - 4].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 3].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite) ||

            // 좌상단 대각선
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            _currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos + 3].IsWhite && board[_currentStone.XPos + 4, _currentStone.YPos + 4].IsWhite) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            _currentStone.YPos >= 1 && _currentStone.YPos < 12 &&
            board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos + 3].IsWhite) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            _currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos + 2].IsWhite) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            _currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            board[_currentStone.XPos - 3, _currentStone.YPos - 3].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos + 1].IsWhite) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            _currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            board[_currentStone.XPos - 4, _currentStone.YPos - 4].IsWhite && board[_currentStone.XPos - 3, _currentStone.YPos - 3].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite) ||

            // 우상단 대각선
            (_currentStone.XPos >= 0 && _currentStone.XPos < 11 &&
            _currentStone.YPos >= 4 && _currentStone.YPos < 15 &&
            board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos - 3].IsWhite && board[_currentStone.XPos + 4, _currentStone.YPos - 4].IsWhite) ||

            (_currentStone.XPos >= 1 && _currentStone.XPos < 12 &&
            _currentStone.YPos >= 3 && _currentStone.YPos < 14 &&
            board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsWhite && board[_currentStone.XPos + 3, _currentStone.YPos - 3].IsWhite) ||

            (_currentStone.XPos >= 2 && _currentStone.XPos < 13 &&
            _currentStone.YPos >= 2 && _currentStone.YPos < 13 &&
            board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsWhite && board[_currentStone.XPos + 2, _currentStone.YPos - 2].IsWhite) ||

            (_currentStone.XPos >= 3 && _currentStone.XPos < 14 &&
            _currentStone.YPos >= 1 && _currentStone.YPos < 11 &&
            board[_currentStone.XPos - 3, _currentStone.YPos + 3].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite && board[_currentStone.XPos + 1, _currentStone.YPos - 1].IsWhite) ||

            (_currentStone.XPos >= 4 && _currentStone.XPos < 15 &&
            _currentStone.YPos >= 0 && _currentStone.YPos < 11 &&
            board[_currentStone.XPos - 4, _currentStone.YPos + 4].IsWhite && board[_currentStone.XPos - 3, _currentStone.YPos + 3].IsWhite && board[_currentStone.XPos - 2, _currentStone.YPos + 2].IsWhite && board[_currentStone.XPos - 1, _currentStone.YPos + 1].IsWhite && board[_currentStone.XPos, _currentStone.YPos].IsWhite)
            )
        {
            //player.IsWin = true;
            return true;
        }

        return false;
    }

    /*
    private void CheckIsOpenThree() { }
    private void CheckIsClosedThree() { }
    private void CheckIsOpenFour() { }
    private void CheckIsClosedFour() { }
    private void CheckIsBrokenFour() { }


    // 놓을 위치가 IsDoubleThree, IsDoubleFour, IsOverline 일 경우 그 위치에 두지 못하게 검사
    private void CheckDoubleThree()
    {
        if (player.IsWhite) return;

        // 묶음 1, 2 = 이 돌을 놔서 만들어지는 연결된 돌 묶음 1, 2
        if (묶음1.IsOpenThree && 묶음2.IsOpenThree)   // 둘다 열린 3일때 (참일때)
        {
            놓을위치.IsDoubleThree = true;  // 쌍삼이기 때문에 놓지 못하게 하기
        }

        // 경우의 수 54개?
    }

    private void CheckDoubleFour()
    {
        if (player.IsWhite) return;

        if ((묶음1.IsOpenFour && 묶음2.IsOpenFour) ||
            (묶음1.IsOpenFour && 묶음2.IsClosedFour) ||
            (묶음1.IsOpenFour && 묶음2.IsBrokenFour) ||
            (묶음1.IsClosedFour && 묶음2.IsOpenFour) ||
            (묶음1.IsClosedFour && 묶음2.IsClosedFour) ||
            (묶음1.IsClosedFour && 묶음2.IsBrokenFour) ||
            (묶음1.IsBrokenFour && 묶음2.IsOpenFour) ||
            (묶음1.IsBrokenFour && 묶음2.IsClosedFour) ||
            (묶음1.IsBrokenFour && 묶음2.IsBrokenFour))
        {
            놓을위치.IsDoubleFour = true;
        }
    }

    private void CheckOverline()
    {
        if (player.IsWhite) return;

        if (묶음1.IsOverline || 묶음2.IsOverline)
        {
            놓을위치.IsOverline = true;
        }
    }
    */
}
