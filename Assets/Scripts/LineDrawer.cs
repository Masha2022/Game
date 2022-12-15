using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public event Action Drawn;

    [SerializeField] 
    private float _pointTrakingThreshold = 0.1f;
    [SerializeField]
    private float _minDistanceBetweenPoints = 0.1f;

    private bool _isDrawingMode;
    private LineRenderer _lineRenderer;
    private float _lastAddedPointTime;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();// получаю lineRenderer
        _lineRenderer.positionCount = 0; // сбросила все точки у lineRenderer
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))//если нажата кнопка мыши
        {
            _lineRenderer.positionCount = 0; //сбросила предыдущие точки у lineRenderer
            _isDrawingMode = true;//режим рисования включен
        }
        if (Input.GetMouseButtonUp(0))//если игрок отпустил кнопку мыши
        {
            Drawn?.Invoke();//вызываю ивент
            _isDrawingMode = false;//режим рисования выключен
        }

        if (!_isDrawingMode)//если сейчас НЕ режим рисования
        {
            return;
        }
        //если режим рисования
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);//луч из камеры в то место, где кликнули мышкой
        if (Physics.Raycast(ray, out var hitInfo))
        {
            TryAddNewPoints(hitInfo.point);
        }
    }

    private void TryAddNewPoints(Vector3 hitPoint)
    {
        //проверяю, достаточно ли прошло времени с момента добавления последней точки
        var isTimeToAddNewPoint = _lastAddedPointTime + _pointTrakingThreshold < Time.time;
        if (!isTimeToAddNewPoint)//если время не накопилось-выхожу
        {
            return;
        }

        if (!IsSomePoint(hitPoint))//проверяю, новая ли точка добавляется
        {
            _lastAddedPointTime = Time.time;//новое время добавления последней точки    
            _lineRenderer.positionCount++;//увеличиваю количество точек 
            _lineRenderer.SetPosition(_lineRenderer.positionCount-1, hitPoint);
        }
        
        
    }

    private bool IsSomePoint(Vector3 hitPoint)
    {
        if (_lineRenderer.positionCount == 0)
        {
            return false;
        }
        //последняя добавленная точка
        var lastPoint = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
        //проверяю расстояние от последней точки до новой точки, меньше ли оно чем _minDistanceBetweenPoints
        return Vector3.Distance(lastPoint, hitPoint) < _minDistanceBetweenPoints;
    }

    public Vector3[] GetPoints()
    {
        Vector3[] points = new Vector3[_lineRenderer.positionCount];//создаю временный массив
        _lineRenderer.GetPositions(points);//возвращаю все точки в массив
        return points;
    }
}
