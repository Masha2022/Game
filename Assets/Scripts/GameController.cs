using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LineDrawer _lineDrawer;
    [SerializeField] private Transform _player;
    [SerializeField] private float _speed = 1;

    private void Awake()
    {
        // подписываюсь на событие Drawn(которое говорит о том, что что-то было нарисовано) 
        //и вызываю при этом событии OnLineDrawn
        _lineDrawer.Drawn += OnLineDrawn;
    }

    private void OnDestroy()
    {
        // отписываюсь от события Drawn
        _lineDrawer.Drawn -= OnLineDrawn;
    }

    private void OnLineDrawn()
    {
        //получаю все нарисованные точки
        var linePoints = _lineDrawer.GetPoints();
        //вызываю корутину MovePlayer
        StartCoroutine(MovePlayer(linePoints)); 
    }

    private IEnumerator MovePlayer(Vector3[] linePoints)
    {
        if (linePoints.Length == 0)
        {
            yield break;
        }
        
        //беру первую точку из массива и задаю ее игроку
        _player.position = linePoints.First();
        //индекс текущей точки
        var currentPointIndex = 0;
        //индекс последней точки
        var lastPointsIndex = linePoints.Length - 1;
        //цикл до тех пор, пока не достигну последнего индекса
        
        while (currentPointIndex < lastPointsIndex)
        {
            //рассчитываю какое расстояние прошел игрок за кадр
            var metersPassedPerFrame = Time.deltaTime * _speed;
            //начинаю движение от текущей точки по всем точкам массива
            currentPointIndex = MoveTowards(currentPointIndex, linePoints, metersPassedPerFrame);
            yield return null;
        }
    }

    private int MoveTowards(int startIndex, Vector3[] linePoints, float metersPassed)
    {
        var currentPointIndex = startIndex;
        while (true)
        {
            //текущее положение
            var currentPoint = _player.position;
            //следующая точка
            var nextPoint = linePoints[currentPointIndex + 1];
            //рассчитываю дистанцию между этими точками
            var distanceToNextPoint = Vector3.Distance(currentPoint, nextPoint);
            
            //если дистанция меньше, чем количество метров которые прошел игрок
            //значит игрок дошел до следующей точки и еще осталось некоторе расстояние
            //которое надо пройти
            if (distanceToNextPoint < metersPassed)
            {
                //задаю игроку позицию
                _player.position = nextPoint;
                //нахожу оставшееся расстояние
                metersPassed -= distanceToNextPoint;
                //сдвигаю индекс на следующую точку
                currentPointIndex++;
                //повторяю 
            }
            else
            {
                //вычисляю процент от расстояния, которое осталось пройти
                var percent = metersPassed / distanceToNextPoint;
                //получаю точку, где остановился игрок
                var currentPosition = Vector3.Lerp(currentPoint, nextPoint, percent);
                //задаю игорк новую точку
                _player.position = currentPosition;
                //возвращаю индекс этой точки
                return currentPointIndex;
            }

            var lastPointIndex = linePoints.Length - 1;
            if (currentPointIndex == lastPointIndex)
            {
                return currentPointIndex;
            }
        }
    }
}