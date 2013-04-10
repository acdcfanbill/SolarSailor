using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SolarSailor
{
    /// <summary>
    /// A third person camera class that uses quaternions for rotation.  It also uses a spring
    /// to dampen motion so the camera doesn't always snap to position behind the ship.
    /// </summary>
    public class ThirdPersonCamera
    {
        private const float DEFAULT_SPRING_CONSTANT = 20.0f;
        private const float DEFAULT_DAMPING_CONSTANT = 10.0f;
        private const float DEFAULT_FOV = 90.0f; //I can't stand small fov's
        private const float DEFAULT_FAR_PLANE = 3000.0f; //may have to adjust this to see far away stuff in space
        private const float DEFAULT_NEAR_PLANE = 1.0f;

        private float springConstant;
        private float dampingConstant;
        private float offsetDistance;
        private float headingDegrees; //these are how you locate the camera circularly
        private float pitchDegrees; //about the ship model it's following
        private float fov;
        private float nearPlane;
        private float farPlane;

        private Vector3 camPos;
        private Vector3 camTarg;
        private Vector3 camUp;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private Vector3 viewDirection;
        private Vector3 velocity;

        private Matrix viewMatrix;
        private Matrix projMatrix;

        private Quaternion orientation;

        public ThirdPersonCamera(float springConstant, float dampingConstant, float nearPlane, float farPlane, float fov)
        {
            this.springConstant = springConstant;
            this.dampingConstant = dampingConstant;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.fov = fov;

            offsetDistance = 0f;
            headingDegrees = 0f;
            pitchDegrees = 0f;

            camPos = Vector3.Zero;
            camTarg = Vector3.Zero;
            camUp = Vector3.Up;

            xAxis = Vector3.UnitX;
            yAxis = Vector3.UnitY;
            zAxis = Vector3.UnitZ;
            viewDirection = Vector3.Forward;

            viewMatrix = Matrix.Identity;
            projMatrix = Matrix.Identity;
            orientation = Quaternion.Identity;
        }
        public ThirdPersonCamera()
            :this(DEFAULT_SPRING_CONSTANT, DEFAULT_DAMPING_CONSTANT, DEFAULT_NEAR_PLANE, DEFAULT_FAR_PLANE, DEFAULT_FOV)
        {
        }

        public void LookAt(Vector3 target)
        {
            this.camTarg = target;
        }
        public void LookAt(Vector3 pos, Vector3 target, Vector3 up)
        {
            this.camPos = pos;
            this.camTarg = target;

            zAxis = pos - target;
            zAxis.Normalize();

            Vector3.Negate(ref zAxis, out viewDirection);

            Vector3.Cross(ref up, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            xAxis.Normalize();
            yAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            Vector3.Dot(ref xAxis, ref pos, out viewMatrix.M41);
            viewMatrix.M41 = -viewMatrix.M41;

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            Vector3.Dot(ref yAxis, ref pos, out viewMatrix.M42);
            viewMatrix.M42 = -viewMatrix.M42;

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            Vector3.Dot(ref zAxis, ref pos, out viewMatrix.M43);
            viewMatrix.M43 = -viewMatrix.M43;

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;

            this.camUp = up;

            Quaternion.CreateFromRotationMatrix(ref viewMatrix, out orientation);

            Vector3 offset = target - pos;

            offsetDistance = offset.Length();
        }

        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this.fov = fovx;
            this.nearPlane = znear;
            this.farPlane = zfar;

            float aspectInv = 1.0f / aspect;
            float e = 1.0f / (float)Math.Tan(MathHelper.ToRadians(fovx) / 2.0f);
            float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
            float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            projMatrix.M11 = xScale;
            projMatrix.M12 = 0.0f;
            projMatrix.M13 = 0.0f;
            projMatrix.M14 = 0.0f;

            projMatrix.M21 = 0.0f;
            projMatrix.M22 = yScale;
            projMatrix.M23 = 0.0f;
            projMatrix.M24 = 0.0f;

            projMatrix.M31 = 0.0f;
            projMatrix.M32 = 0.0f;
            projMatrix.M33 = (zfar + znear) / (znear - zfar);
            projMatrix.M34 = -1.0f;

            projMatrix.M41 = 0.0f;
            projMatrix.M42 = 0.0f;
            projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            projMatrix.M44 = 0.0f;
        }

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        public void Rotate(float headingDegrees, float pitchDegrees)
        {
            this.headingDegrees = -headingDegrees;
            this.pitchDegrees = -pitchDegrees;
        }

        /// <summary>
        /// This method must be called once every frame to update the internal
        /// state of the ThirdPersonCamera object.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public void Update(GameTime gameTime)
        {
            float elapsedTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateOrientation(elapsedTimeSec);

            UpdateViewMatrix(elapsedTimeSec);
        }

        private void UpdateOrientation(float elapsedTimeSec)
        {
            headingDegrees *= elapsedTimeSec;
            pitchDegrees *= elapsedTimeSec;

            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);
            Quaternion rotation = Quaternion.Identity;

            if (heading != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref camUp, heading, out rotation);
                Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
            }

            if (pitch != 0.0f)
            {
                Vector3 worldXAxis = Vector3.UnitX;
                Quaternion.CreateFromAxisAngle(ref worldXAxis, pitch, out rotation);
                Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            }
        }

        private void UpdateViewMatrix(float elapsedTimeSec)
        {
            Matrix.CreateFromQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            // Calculate the new camera position. The 'idealPosition' is where
            // the camera should be positioned. The camera should be positioned
            // directly behind the target at the required offset distance. What
            // we're doing here is rather than have the camera immediately snap
            // to the 'idealPosition' we slowly move the camera towards the
            // 'idealPosition' using a spring system.
            //
            // References:
            //  Stone, Jonathan, "Third-Person Camera Navigation," Game Programming
            //    Gems 4, Andrew Kirmse, Editor, Charles River Media, Inc., 2004.

            Vector3 idealPosition = camTarg + zAxis * offsetDistance;
            Vector3 displacement = camPos - idealPosition;
            Vector3 springAcceleration = (-springConstant * displacement) -
                                         (dampingConstant * velocity);

            velocity += springAcceleration * elapsedTimeSec;
            camPos += velocity * elapsedTimeSec;

            // The view matrix is always relative to the camera's current position.
            // Since a spring system is being used here 'eye' will be relative to
            // 'idealPosition'. When the camera is no longer being moved 'eye' will
            // become the same as 'idealPosition'. The local x, y, and z axes that
            // were extracted from the camera's orientation 'orienation' is correct
            // for the 'idealPosition' only. We need to recompute these axes so
            // that they're relative to 'eye'. Once that's done we can use those
            // axes to reconstruct the view matrix.

            zAxis = camPos - camTarg;
            zAxis.Normalize();

            Vector3.Negate(ref zAxis, out viewDirection);

            Vector3.Cross(ref camUp, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            yAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            Vector3.Dot(ref xAxis, ref camPos, out viewMatrix.M41);
            viewMatrix.M41 = -viewMatrix.M41;

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            Vector3.Dot(ref yAxis, ref camPos, out viewMatrix.M42);
            viewMatrix.M42 = -viewMatrix.M42;

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            Vector3.Dot(ref zAxis, ref camPos, out viewMatrix.M43);
            viewMatrix.M43 = -viewMatrix.M43;

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projMatrix; }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
        }
    }
}
