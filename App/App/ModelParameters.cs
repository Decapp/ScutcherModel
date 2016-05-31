﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class ModelParameters
    {
        //Параметры пряди

        public double?[] threadWeightRasp; ///Коэффициенты распределения массы

        public double expectedValueLength; ///Мат. ожидание
        public double varianceLength; ///Дисперсия


        public double? threadTopDiameter; ///Диаметр нити у зажима
        public double? threadMidDiameter; ///Диаметр нити в середине
        public double? threadBotDiameter; ///Диаметр нити на конце


        public double threadYoungModul; ///Модуль Юнга
        public double threadPosition; ///Положение по X точки зажима
        public double threadFriction; ///Коэффициент трения нити
        public double threadHard; ///Коэффициент изгибной жесткости
        public double? threadPlotn; ///Плотность нити

        public double[] weightLength; ///Коэффициенты функции
        ///зависимости массы от длины стебля

        public double expectedValueOffset; ///Мат. ожидание
        public double varianceOffset; ///Дисперсия


        public double expectedValueAngle; ///Мат. ожидание
        public double varianceAngle; ///Дисперсия


        public int threadPointCount; ///Количество точек

        //Параметры зажимного механизма

        public double clampLength; ///Длина линии сопряжения
        public double beltDistance; ///Расстояние между ремнями

        //Параметры барабанов

        public double[,] rollParameter;

        //Параметры моделирования

        public double dt; ///время
        public int yarnCount; ///Количество прядок
        public double windage; ///Сопротивление воздуха



        public double clampForce; ///Сила зажима




        public ModelParameters(double?[] _threadWeigthRasp,
            double _expectedValueLength,
            double _varianceLength,
            double? _threadTopDiameter,
            double? _threadMidDiameter,
            double? _threadBotDiameter,
            double _threadYoungModul,
            double _threadPosition,
            double _threadFriction,
            double _threadHard,

            double? _threadPlotn,

            double _expectedValueOffset,
            double _varianceOffset,
            double _expectedValueAngle,
            double _varianceAngle,
            double[,] _rollParameter,
            double _clampLength,
            double _beltDistance,
            double _dt,
            int _threadPointCount,
            int _yarnCount,
            double _windage,
            double _clampForce,
            double[] _weightLength)
        {
            this.threadWeightRasp = _threadWeigthRasp;

            this.expectedValueLength = _expectedValueLength;
            this.varianceLength = _varianceLength;

            this.threadYoungModul = _threadYoungModul;
            this.threadPosition = _threadPosition;
            this.threadFriction = _threadFriction;
            this.threadHard = _threadHard;
            this.threadPlotn = _threadPlotn;

            this.weightLength = _weightLength;

            this.threadTopDiameter = _threadTopDiameter;
            this.threadMidDiameter = _threadMidDiameter;
            this.threadBotDiameter = _threadBotDiameter;

            this.expectedValueOffset = _expectedValueOffset;
            this.varianceOffset = _varianceOffset;

            this.expectedValueAngle = _expectedValueAngle;
            this.varianceAngle = _varianceAngle;

            this.rollParameter = _rollParameter;

            this.threadPointCount = _threadPointCount;

            this.clampLength = _clampLength;
            this.beltDistance = _beltDistance;

            this.dt = _dt;
            this.yarnCount = _yarnCount;
            this.windage = _windage;

            this.clampForce = _clampForce;
        }
    }
}
