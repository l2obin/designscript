﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Geometry;

namespace DSGeometry
{
    class PlaneEntity : GeometryEntity, IPlaneEntity
    {
        internal PlaneEntity()
        {
            this.Origin = new PointEntity();
            this.Normal = DsVector.ByCoordinates(0, 0, 1);
            this.contextCS = new CoordinateEntity();
        }
        private ICoordinateSystemEntity contextCS = null;
        public ICoordinateSystemEntity ContextCS
        {
            get
            {
                if (contextCS == null)
                    contextCS = new CoordinateEntity();
                return contextCS;
            }
            protected set { contextCS = value; }
        }
        public ICoordinateSystemEntity GetCoordinateSystem()
        {
            return ContextCS;
        }

        public ILineEntity IntersectWith(IPlaneEntity perpPlane)
        {
            return new LineEntity();
        }

        public IVector Normal
        {
            get;
            protected set;
        }

        public IPointEntity Origin
        {
            get;
            protected set;
        }

        public IPointEntity Project(IPointEntity iPointEntity, IVector direction)
        {
            return iPointEntity;
        }

        public void Update(IPointEntity origin, IVector normal)
        {
            this.Origin = origin;
            this.Normal = normal;
        }
    }
}
