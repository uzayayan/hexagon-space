using System;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardController : BaseController
{
    #region Public Fields

    public Action Moved;
    public Action<int> Matched;

    #endregion
    #region Private Fields

    private Board board;
    private GameSettings gameSettings;
    private List<SlotController> slotControllers = new List<SlotController>();
    private List<HexagonController> hexagonControllers = new List<HexagonController>();
    private List<List<SlotController>> seperateSlotControllers = new List<List<SlotController>>();

    private bool isGravity;
    private bool isCheckingBoard;

    #endregion
    
    /// <summary>
    /// This function helper for initialize controller.
    /// </summary>
    /// 0 = Board
    public override void Initialize(params object[] parameters)
    {
        board = (Board) parameters[0];
        gameSettings = GameManager.Instance.GetGameSettings();
        
        //Initial set for bomb of next point.
        board.NextBombPoint = board.NextBombPoint == 0 ? gameSettings.BombInitialPoint : board.NextBombPoint;

        _= InitializeBoardData();
        
        base.Initialize(parameters);
    }

    /// <summary>
    /// This function helper for initialize board data.
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeBoardData()
    {
        Vector2 gridSize = gameSettings.GridSize;

        if (board.Hexagons != null) //Cached Board Data
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Hexagon targetHexagon = board.Hexagons.Single(hexagon => hexagon.GetCoordinate() == new Coordinate(x, y));
                    targetHexagon.Color = gameSettings.Colors[targetHexagon.ColorId];

                    if (targetHexagon.GetType() == typeof(Bomb))
                    {
                        CreateBomb((Bomb) targetHexagon, CreateSlot(x, y));
                        continue;
                    }
                    
                    if (targetHexagon.GetType() == typeof(Hexagon))
                    {
                        CreateHexagon(targetHexagon, CreateSlot(x, y));
                        continue;
                    }
                }
            }
        }
        else //New Board Data
        {
            while (true)
            {
                ClearBoardData();
                
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int x = 0; x < gridSize.x; x++)
                    {
                        CreateHexagon(null, CreateSlot(x, y));
                    }
                }

                bool isReadyToPlay =  !await CheckHexagons(hexagonControllers, false, true);
               
                if(isReadyToPlay)
                    break;
            }
        }
        
        for (int i = 0; i < gridSize.x; i++) //Separate slot by vertical coordinate.
        {
            seperateSlotControllers.Add(slotControllers.Where(x=> x.GetCoordinate().x == i).ToList());
        }
    }

    /// <summary>
    /// This function helper for crate Hexagon Controller by coordinate.
    /// </summary>
    private SlotController CreateSlot(int x, int y)
    {
        Coordinate coordinate = new Coordinate(x, y);
        Vector2 targetPosition = Vector2.zero;
        targetPosition.x += (coordinate.x * CommonTypes.HEXAGON_WIDTH * 0.75F);
        targetPosition.y -= coordinate.x % 2 == 0 ? coordinate.y * CommonTypes.HEXAGON_HEIGHT : CommonTypes.HEXAGON_HEIGHT / 2 + CommonTypes.HEXAGON_HEIGHT * coordinate.y;

        Slot slot = new Slot(coordinate);
        SlotController slotController = Instantiate(gameSettings.SlotPrefab, InterfaceManager.Instance.GetPlayArea(), false);
        slotController.transform.localPosition = targetPosition;

        slotController.name = $"Slot_({x}, {y})";
        slotController.Initialize(slot);
        
        slotControllers.Add(slotController);

        return slotController;
    }

    /// <summary>
    /// This function helper for create random Hexagon.
    /// </summary>
    private HexagonController CreateHexagon(Hexagon hexagon, SlotController slotController, bool isOut = false)
    {
        if (hexagon == null)
        {
            bool isBonus = Random.Range(0, 10) == 5;
            int colorId = Random.Range(0, gameSettings.Colors.Length);
            
            hexagon = new Hexagon(colorId, gameSettings.Colors[colorId], isBonus);
        }
        
        HexagonController hexagonController = Instantiate(gameSettings.HexagonPrefab, slotController.transform, false);

        if (isOut)
            hexagonController.transform.position = slotController.transform.TransformVector(new Vector3(slotController.transform.localPosition.x, InterfaceManager.Instance.GetOutScreenYAxis(CommonTypes.HEXAGON_HEIGHT), 0));

        hexagonController.Initialize(hexagon, slotController);
        hexagonControllers.Add(hexagonController);
        
        Debug.Log($"Hexagon is Created. ColorId : {hexagonController.GetColorId()} Coordinate : {hexagonController.GetCoordinate()}");

        return hexagonController;
    }
    
    /// <summary>
    /// This function helper for create random Hexagon.
    /// </summary>
    private BombController CreateBomb(Bomb bomb, SlotController slotController, bool isOut = false)
    {
        if (bomb == null)
        {
            int colorId = Random.Range(0, gameSettings.Colors.Length);
            
            bomb = new Bomb(colorId, gameSettings.Colors[colorId], false, gameSettings.BombInitialCounter);
        }
        
        BombController bombController = Instantiate(gameSettings.BombPrefab, slotController.transform, false);

        if (isOut)
            bombController.transform.position = slotController.transform.TransformVector(new Vector3(slotController.transform.localPosition.x, InterfaceManager.Instance.GetOutScreenYAxis(CommonTypes.HEXAGON_HEIGHT), 0));

        bombController.Initialize(bomb, slotController);
        hexagonControllers.Add(bombController);
        
        Debug.Log($"Bomb is Created. ColorId : {bombController.GetColorId()} Coordinate : {bombController.GetCoordinate()} Counter : {bombController.GetCounter()}");
        
        return bombController;
    }

    /// <summary>
    /// This function returns Hexagon Controller by coordinate.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="canNull"></param>
    /// <returns></returns>
    public HexagonController GetHexagonControllerByCoordinate(Coordinate coordinate, bool canNull = false)
    {
        HexagonController targetHexagonController = hexagonControllers.SingleOrDefault(x=> x.GetCoordinate() == coordinate);

        if (targetHexagonController == null && !canNull)
        {
            Debug.LogError($"Target Hexagon Controller is Null! Coordinate : {coordinate}");
        }
        
        return targetHexagonController;
    }

    /// <summary>
    /// This function returns Slot Controller by coordinate.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public SlotController GetSlotControllerByCoordinate(Coordinate coordinate)
    {
        SlotController targetSlotController = slotControllers.SingleOrDefault(x=> x.GetCoordinate() == coordinate);

        if (targetSlotController == null)
        {
            Debug.LogError($"Target Slot Controller is Null! Coordinate : {coordinate}");
        }

        return targetSlotController;
    }

    /// <summary>
    /// This function helper check all board for find completed hexagons.
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> CheckHexagons(List<HexagonController> targetHexagonControllers, bool fireMovedEvent = false, bool isJustCheck = false)
    {
        isCheckingBoard = true;
        Debug.Log("Check Hexagons Progress Started.");

        if (targetHexagonControllers == null)
        {
            Debug.LogError("Target Hexagon Controllers is null!");
            return false;
        }
        
        bool isAnyChangeOnBoard = false;
        
        for (int i = 0; i < targetHexagonControllers.Count; i++)
        {
            foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
            {
                if (targetHexagonControllers[i] == null)
                    continue;

                int colorId = targetHexagonControllers[i].GetColorId();
                List<HexagonController> neighboursHexagonControllers = targetHexagonControllers[i].GetNeighboursHexagonControllers(direction);

                if (neighboursHexagonControllers.Count < 2)
                    continue;

                bool isCompleted = neighboursHexagonControllers.All(x => x.GetColorId() == colorId);

                if (isJustCheck && isCompleted)
                    return true;

                if (isCompleted)
                {
                    int score = 0;
                    Vector3 centerPoint = MathUtils.CalculateCenterPoint(targetHexagonControllers[i].GetSlotPosition(), neighboursHexagonControllers.Select(x => x.GetSlotPosition()).ToArray());

                    foreach (HexagonController completedHexagonController in neighboursHexagonControllers.Append(targetHexagonControllers[i]))
                    {
                        score += gameSettings.ScoreAdditive * (completedHexagonController.IsBonus() ? gameSettings.ScoreBonusMultiplier : 1);
                        Destroy(completedHexagonController.gameObject);
                    }

                    InterfaceManager.Instance.DrawFloatingText($"+{score}", centerPoint);
                    InterfaceManager.Instance.DrawDebrisParticle(targetHexagonControllers[i].GetColor(), centerPoint);

                    DOTween.Kill(CommonTypes.ANCHOR_POINT_TWEEN_KEY);
                    SoundManager.Instance.Play(SoundType.Match);

                    isAnyChangeOnBoard = true;
                    Matched?.Invoke(score);

                    Debug.Log($"Some Hexagons Completed! Score : {score} Color ID : {colorId}");

                    await UniTask.WaitForEndOfFrame();
                }
            }
        }
        
        if (isAnyChangeOnBoard)
        {
            if (fireMovedEvent)
            {
                Moved?.Invoke();
            }
            
            hexagonControllers.RemoveAll(x => x == null);
            await Gravity();
        }

        isCheckingBoard = false;
        Debug.Log("Check Hexagons Progress Completed.");

        return isAnyChangeOnBoard;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            _ = CheckGameOver();
        }
    }

    /// <summary>
    /// This function returns true if there are any moves left.
    /// </summary>
    /// <returns></returns>
    public async UniTask CheckGameOver()
    {
        await UniTask.Yield();
        
        foreach (HexagonController hexagonController in hexagonControllers)
        {
            bool isAnyColorMatch = false;
            HexagonController otherHexagonController = null;

            foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
            {
                List<HexagonController> neighboursHexagonControllers = hexagonController.GetNeighboursHexagonControllers(direction);

                foreach (HexagonController neighboursHexagonController in neighboursHexagonControllers)
                {
                    if (hexagonController.GetColorId() == neighboursHexagonController.GetColorId())
                    {
                        otherHexagonController = neighboursHexagonController;
                        isAnyColorMatch = true;
                        break;
                    }
                }
                
                if(isAnyColorMatch)
                    break;
            }
            
            if(!isAnyColorMatch)
                continue;
            
            if(otherHexagonController == null)
                continue;

            Coordinate[] extractHexagonCoordinates =
            {
                hexagonController.GetCoordinate(),
                otherHexagonController.GetCoordinate()
            };

            List<HexagonController> possibleHexagonControllers = new List<HexagonController>();
            
            Coordinate currentHexagonCoordinate = hexagonController.GetCoordinate();
            Coordinate otherHexagonCoordinate = otherHexagonController.GetCoordinate();

            if (currentHexagonCoordinate.x == otherHexagonCoordinate.x)
            {
                if (currentHexagonCoordinate.x % 2 == 0)
                {
                    HexagonController targetHexagonController = currentHexagonCoordinate.y < otherHexagonCoordinate.y ? hexagonController : otherHexagonController;
                    
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(targetHexagonController.GetCoordinate() + new Coordinate(1, 0), true));
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(targetHexagonController.GetCoordinate() + new Coordinate(-1, 0), true));
                }
                else
                {
                    HexagonController targetHexagonController = currentHexagonCoordinate.y > otherHexagonCoordinate.y ? hexagonController : otherHexagonController;
                    
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(targetHexagonController.GetCoordinate() + new Coordinate(1, 0), true));
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(targetHexagonController.GetCoordinate() + new Coordinate(-1, 0), true));
                }
            }
            else
            {
                if (currentHexagonCoordinate.y == otherHexagonCoordinate.y)
                {
                    HexagonController evenHexagonController = currentHexagonCoordinate.x % 2 == 0 ? hexagonController : otherHexagonController;
                    HexagonController oddHexagonController = currentHexagonCoordinate.x % 2 != 0 ? hexagonController : otherHexagonController;
                    
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(oddHexagonController.GetCoordinate() + new Coordinate(0, -1), true));
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(evenHexagonController.GetCoordinate() + new Coordinate(0, 1), true));
                }
                else
                {
                    HexagonController smallerYAxisHexagonController = currentHexagonCoordinate.y < otherHexagonCoordinate.y ? hexagonController : otherHexagonController;
                    HexagonController greaterYAxisHexagonController = currentHexagonCoordinate.y > otherHexagonCoordinate.y ? hexagonController : otherHexagonController;
                    
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(greaterYAxisHexagonController.GetCoordinate() + new Coordinate(0, -1), true));
                    possibleHexagonControllers.Add(GetHexagonControllerByCoordinate(smallerYAxisHexagonController.GetCoordinate() + new Coordinate(0, 1), true));
                }
            }

            possibleHexagonControllers.RemoveAll(x => x == null);
            
            foreach (HexagonController possibleHexagonController in possibleHexagonControllers)
            {
                if (possibleHexagonController.CanBeColor(hexagonController.GetColorId(), extractHexagonCoordinates))
                {
                    Debug.Log($"Move Caught. Coordinate : {possibleHexagonController.GetCoordinate()}");

                    return;
                }
            }
        }
        
        Debug.Log("Player Has No More Moves to Play!");
        GameManager.Instance.GameOver(1);
    }

    /// <summary>
    /// This function helper for shift some hexagons like gravity..
    /// </summary>
    /// <returns></returns>
    public async UniTask Gravity()
    {
        isGravity = true;
        Debug.Log("Gravity Progress Started.");

        foreach (List<SlotController> verticalSlots in seperateSlotControllers)
        {
            int emptyCount = 0;
            int interactedCount = 1;

            foreach (SlotController slotController in verticalSlots.AsEnumerable().Reverse().ToList())
            {
                if (slotController.GetHexagonController() == null)
                {
                    emptyCount++;
                    continue;
                }
                
                if(emptyCount == 0)
                    continue;

                HexagonController hexagonController = slotController.GetHexagonController();
                SlotController oldSlotController = hexagonController.GetSlotController();
                SlotController targetSlotController = GetSlotControllerByCoordinate(new Coordinate(hexagonController.GetCoordinate().x, hexagonController.GetCoordinate().y + emptyCount));
                
                if(targetSlotController == null)
                    continue;
                
                oldSlotController.SetHexagonController(null);
                hexagonController.SetSlotController(targetSlotController);
                ((HexagonView) hexagonController.GetView()).DrawDefaultLayer(true, (float) interactedCount / 10);

                interactedCount++;
            }
        }

        await UniTask.WaitUntil(() => !DOTween.IsTweening(CommonTypes.HEXAGON_GRAVITY_TWEEN_KEY));

        FillEmptySlots();

        await UniTask.WaitUntil(() => !DOTween.IsTweening(CommonTypes.HEXAGON_GRAVITY_TWEEN_KEY));

        await CheckHexagons(hexagonControllers);
        await CheckGameOver(); 
        
        isGravity = false;
        Debug.Log("Gravity Progress Completed.");
    }
    
    /// <summary>
    /// This function helper for create new hexagons to empty slot.
    /// </summary>
    public void FillEmptySlots()
    {
        Debug.Log("Fill Empty Slots Progress Started.");

        foreach (List<SlotController> verticalSlots in seperateSlotControllers)
        {
            List<SlotController> emptySlots = new List<SlotController>();
            
            foreach (SlotController slotController in verticalSlots)
            {
                if(slotController.GetHexagonController() != null)
                    break;

                emptySlots.Add(slotController);
            }
            
            if(emptySlots.Count == 0)
                continue;

            Debug.Log($"{emptySlots.Count} Empty Slot Detected.");
            emptySlots.Reverse();

            bool spawnBomb = false;
            int randomBombSlot = 0;

            if (GameManager.Instance.GetScore() >= board.NextBombPoint)
            {
                board.NextBombPoint += gameSettings.BombInitialPoint;
                randomBombSlot = Random.Range(0, emptySlots.Count);
                
                spawnBomb = true;
                
                Debug.Log($"Bomb Time! Next Bomb Point : {board.NextBombPoint}");
            }
            
            for (int i = 0; i < emptySlots.Count; i++)
            {
                if (spawnBomb && i == randomBombSlot)
                {
                    BombController createdBomb = CreateBomb(null, emptySlots[i], true);
                    ((BombView) createdBomb.GetView()).DrawDefaultLayer(true, (float) i / 10);
                    spawnBomb = false;
                }
                else
                {
                    HexagonController createdHexagon = CreateHexagon(null, emptySlots[i], true);
                    ((HexagonView) createdHexagon.GetView()).DrawDefaultLayer(true, (float) i / 10);
                }
            }
        }
        
        Debug.Log("Fill Empty Slots Progress Completed.");
    }

    /// <summary>
    /// This function return true if board is locked.
    /// </summary>
    /// <returns></returns>
    public bool IsLocked()
    {
        if (GameManager.IsGameOver())
        {
            Debug.Log("Board is Locked. Reason : The Game is Over!");
            return true;
        }
        
        if (isGravity)
        {
            Debug.Log("Board is Locked. Reason : Gravity Progress still running.");
            return true;
        }
        
        if (isCheckingBoard)
        {
            Debug.Log("Board is Locked. Reason : Check Hexagons progress still running.");
            return true;
        }

        if (DOTween.IsTweening(CommonTypes.ANCHOR_POINT_TWEEN_KEY))
        {
            Debug.Log($"Board is Locked. Reason : {CommonTypes.ANCHOR_POINT_TWEEN_KEY} is still in progress.");
            return true;
        }

        if (DOTween.IsTweening(CommonTypes.HEXAGON_GRAVITY_TWEEN_KEY))
        {
            Debug.Log($"Board is Locked. Reason : {CommonTypes.HEXAGON_GRAVITY_TWEEN_KEY} is still in progress.");
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// This function helper for save data on this component.
    /// </summary>
    public void SaveData()
    {
        if (!GameManager.IsGameOver())
        {
            board.Hexagons = hexagonControllers.Select(x => x.GetHexagon()).ToList();
            board.SaveData();
            
            Debug.Log("Board Data is Saved.");
        }
    }
    
    /// <summary>
    /// This function called when game pause state changed.
    /// </summary>
    /// <param name="pauseStatus"></param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
            SaveData();
    }

    /// <summary>
    /// This function helper for clear board data.
    /// </summary>
    private void ClearBoardData()
    {
        foreach (HexagonController hexagonController in hexagonControllers)
        {
            Destroy(hexagonController.gameObject);
        }
        
        foreach (SlotController slotController in slotControllers)
        {
            Destroy(slotController.gameObject);
        }
        
        slotControllers.Clear();
        hexagonControllers.Clear();
        seperateSlotControllers.Clear();
    }

    /// <summary>
    /// This function called when this component destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        SaveData();
        base.OnDestroy();
    }
}
