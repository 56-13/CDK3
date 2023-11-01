using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CDK.Drawing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x3 : IEquatable<Matrix3x3>
    {
        public static readonly Matrix3x3 Zero = new Matrix3x3();
        public static readonly Matrix3x3 Identity = new Matrix3x3() { M11 = 1, M22 = 1, M33 = 1 };

        public float M11;
        public float M12;
        public float M13;
        public float M21;
        public float M22;
        public float M23;
        public float M31;
        public float M32;
        public float M33;

        public Matrix3x3(float value)
        {
            M11 = M12 = M13 =
            M21 = M22 = M23 =
            M31 = M32 = M33 = value;
        }

        public Matrix3x3(float M11, float M12, float M13,
            float M21, float M22, float M23,
            float M31, float M32, float M33)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13;
            this.M21 = M21; this.M22 = M22; this.M23 = M23;
            this.M31 = M31; this.M32 = M32; this.M33 = M33;
        }

        public Matrix3x3(float[] values)
        {
            M11 = values[0];
            M12 = values[1];
            M13 = values[2];

            M21 = values[3];
            M22 = values[4];
            M23 = values[5];

            M31 = values[6];
            M32 = values[7];
            M33 = values[8];
        }

        public Vector3 Row1
        {
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
            get => new Vector3(M11, M12, M13);
        }

        public Vector3 Row2
        {
            set 
            { 
                M21 = value.X; 
                M22 = value.Y; 
                M23 = value.Z; 
            }
            get => new Vector3(M21, M22, M23);
        }

        public Vector3 Row3
        {
            set 
            { 
                M31 = value.X; 
                M32 = value.Y; 
                M33 = value.Z; 
            }
            get => new Vector3(M31, M32, M33);
        }

        public Vector3 Column1
        {
            set
            {
                M11 = value.X;
                M21 = value.Y;
                M31 = value.Z;
            }
            get => new Vector3(M11, M21, M31);
        }

        public Vector3 Column2
        {
            set
            {
                M12 = value.X;
                M22 = value.Y;
                M32 = value.Z;
            }
            get => new Vector3(M12, M22, M32);
        }

        public Vector3 Column3
        {
            set
            {
                M13 = value.X;
                M23 = value.Y;
                M33 = value.Z;
            }
            get => new Vector3(M13, M23, M33);
        }

        public Vector3 ScaleVector
        {
            set
            {
                M11 = value.X;
                M22 = value.Y;
                M33 = value.Z;
            }
            get => new Vector3(M11, M22, M33);
        }

        public bool IsIdentity => Equals(Identity);

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M21;
                    case 4: return M22;
                    case 5: return M23;
                    case 6: return M31;
                    case 7: return M32;
                    case 8: return M33;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x3 run from 0 to 8, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M21 = value; break;
                    case 4: M22 = value; break;
                    case 5: M23 = value; break;
                    case 6: M31 = value; break;
                    case 7: M32 = value; break;
                    case 8: M33 = value; break;
                }
                throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x3 run from 0 to 8, inclusive.");
            }
        }

        public float this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 2, inclusive.");

                return this[(row * 3) + column];
            }

            set
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 2, inclusive.");

                this[(row * 3) + column] = value;
            }
        }

        public float Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31 - M12 * M21 * M33 - M11 * M23 * M32;
        }

        public void Invert() => Invert(this, out this);
        public void Transpose() => Transpose(this, out this);
        public void Orthogonalize() => Orthogonalize(this, out this);
        public void Orthonormalize() => Orthonormalize(this, out this);
        public void DecomposeQR(out Matrix3x3 Q, out Matrix3x3 R)
        {
            var temp = this;
            temp.Transpose();
            Orthonormalize(temp, out Q);
            Q.Transpose();

            R = new Matrix3x3
            {
                M11 = Vector3.Dot(Q.Column1, Column1),
                M12 = Vector3.Dot(Q.Column1, Column2),
                M13 = Vector3.Dot(Q.Column1, Column3),

                M22 = Vector3.Dot(Q.Column2, Column2),
                M23 = Vector3.Dot(Q.Column2, Column3),

                M33 = Vector3.Dot(Q.Column3, Column3)
            };
        }

        public void DecomposeLQ(out Matrix3x3 L, out Matrix3x3 Q)
        {
            Orthonormalize(this, out Q);

            L = new Matrix3x3
            {
                M11 = Vector3.Dot(Q.Row1, Row1),

                M21 = Vector3.Dot(Q.Row1, Row2),
                M22 = Vector3.Dot(Q.Row2, Row2),

                M31 = Vector3.Dot(Q.Row1, Row3),
                M32 = Vector3.Dot(Q.Row2, Row3),
                M33 = Vector3.Dot(Q.Row3, Row3)
            };
        }

        public bool Decompose(out Vector3 scale, out Quaternion rotation)
        {
            scale.X = (float)Math.Sqrt((M11 * M11) + (M12 * M12) + (M13 * M13));
            scale.Y = (float)Math.Sqrt((M21 * M21) + (M22 * M22) + (M23 * M23));
            scale.Z = (float)Math.Sqrt((M31 * M31) + (M32 * M32) + (M33 * M33));

            if (MathUtil.NearZero(scale.X) || MathUtil.NearZero(scale.Y) || MathUtil.NearZero(scale.Z))
            {
                rotation = Quaternion.Identity;
                return false;
            }

            var rm = new Matrix4x4
            {
                M11 = M11 / scale.X,
                M12 = M12 / scale.X,
                M13 = M13 / scale.X,

                M21 = M21 / scale.Y,
                M22 = M22 / scale.Y,
                M23 = M23 / scale.Y,

                M31 = M31 / scale.Z,
                M32 = M32 / scale.Z,
                M33 = M33 / scale.Z,

                M44 = 1
            };

            rotation = Quaternion.CreateFromRotationMatrix(rm);
            return true;
        }

        public bool DecomposeUniformScale(out float scale, out Quaternion rotation)
        {
            scale = (float)Math.Sqrt((M11 * M11) + (M12 * M12) + (M13 * M13));
            var inv_scale = 1 / scale;

            if (Math.Abs(scale) < MathUtil.ZeroTolerance)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            var rm = new Matrix4x4
            {
                M11 = M11 * inv_scale,
                M12 = M12 * inv_scale,
                M13 = M13 * inv_scale,

                M21 = M21 * inv_scale,
                M22 = M22 * inv_scale,
                M23 = M23 * inv_scale,

                M31 = M31 * inv_scale,
                M32 = M32 * inv_scale,
                M33 = M33 * inv_scale,

                M44 = 1
            };

            rotation = Quaternion.CreateFromRotationMatrix(rm);
            return true;
        }

        public void ExchangeRows(int firstRow, int secondRow)
        {
            if (firstRow < 0)
                throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be greater than or equal to zero.");
            if (firstRow > 2)
                throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be less than or equal to two.");
            if (secondRow < 0)
                throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be greater than or equal to zero.");
            if (secondRow > 2)
                throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be less than or equal to two.");

            if (firstRow == secondRow)
                return;

            var temp0 = this[secondRow, 0];
            var temp1 = this[secondRow, 1];
            var temp2 = this[secondRow, 2];

            this[secondRow, 0] = this[firstRow, 0];
            this[secondRow, 1] = this[firstRow, 1];
            this[secondRow, 2] = this[firstRow, 2];

            this[firstRow, 0] = temp0;
            this[firstRow, 1] = temp1;
            this[firstRow, 2] = temp2;
        }

        public void ExchangeColumns(int firstColumn, int secondColumn)
        {
            if (firstColumn < 0)
                throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be greater than or equal to zero.");
            if (firstColumn > 2)
                throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be less than or equal to two.");
            if (secondColumn < 0)
                throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be greater than or equal to zero.");
            if (secondColumn > 2)
                throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be less than or equal to two.");

            if (firstColumn == secondColumn)
                return;

            var temp0 = this[0, secondColumn];
            var temp1 = this[1, secondColumn];
            var temp2 = this[2, secondColumn];

            this[0, secondColumn] = this[0, firstColumn];
            this[1, secondColumn] = this[1, firstColumn];
            this[2, secondColumn] = this[2, firstColumn];

            this[0, firstColumn] = temp0;
            this[1, firstColumn] = temp1;
            this[2, firstColumn] = temp2;
        }

        public float[] ToArray() => new float[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };

        public static void Add(in Matrix3x3 left, in Matrix3x3 right, out Matrix3x3 result)
        {
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
        }

        public static Matrix3x3 Add(in Matrix3x3 left, in Matrix3x3 right)
        {
            Add(left, right, out var result);
            return result;
        }

        public static void Subtract(in Matrix3x3 left, in Matrix3x3 right, out Matrix3x3 result)
        {
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
        }

        public static Matrix3x3 Subtract(in Matrix3x3 left, in Matrix3x3 right)
        {
            Subtract(left, right, out var result);
            return result;
        }

        public static void Multiply(in Matrix3x3 left, float right, out Matrix3x3 result)
        {
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M13 = left.M13 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            result.M23 = left.M23 * right;
            result.M31 = left.M31 * right;
            result.M32 = left.M32 * right;
            result.M33 = left.M33 * right;
        }

        public static Matrix3x3 Multiply(in Matrix3x3 left, float right)
        {
            Multiply(left, right, out var result);
            return result;
        }

        public static void Multiply(in Matrix3x3 left, in Matrix3x3 right, out Matrix3x3 result)
        {
            var temp = new Matrix3x3
            {
                M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31),
                M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32),
                M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33),
                M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31),
                M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32),
                M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33),
                M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31),
                M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32),
                M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33)
            };
            result = temp;
        }

        public static Matrix3x3 Multiply(in Matrix3x3 left, in Matrix3x3 right)
        {
            Multiply(left, right, out var result);
            return result;
        }

        public static void Divide(in Matrix3x3 left, float right, out Matrix3x3 result)
        {
            var inv = 1 / right;

            result.M11 = left.M11 * inv;
            result.M12 = left.M12 * inv;
            result.M13 = left.M13 * inv;
            result.M21 = left.M21 * inv;
            result.M22 = left.M22 * inv;
            result.M23 = left.M23 * inv;
            result.M31 = left.M31 * inv;
            result.M32 = left.M32 * inv;
            result.M33 = left.M33 * inv;
        }

        public static Matrix3x3 Divide(in Matrix3x3 left, float right)
        {
            Divide(left, right, out var result);
            return result;
        }

        public static void Divide(in Matrix3x3 left, in Matrix3x3 right, out Matrix3x3 result)
        {
            result.M11 = left.M11 / right.M11;
            result.M12 = left.M12 / right.M12;
            result.M13 = left.M13 / right.M13;
            result.M21 = left.M21 / right.M21;
            result.M22 = left.M22 / right.M22;
            result.M23 = left.M23 / right.M23;
            result.M31 = left.M31 / right.M31;
            result.M32 = left.M32 / right.M32;
            result.M33 = left.M33 / right.M33;
        }

        public static Matrix3x3 Divide(in Matrix3x3 left, in Matrix3x3 right)
        {
            Divide(left, right, out var result);
            return result;
        }

        public static void Exponent(in Matrix3x3 value, int exponent, out Matrix3x3 result)
        {
            if (exponent < 0) throw new ArgumentOutOfRangeException("exponent", "The exponent can not be negative.");

            if (exponent == 0)
            {
                result = Identity;
                return;
            }

            if (exponent == 1)
            {
                result = value;
                return;
            }

            var identity = Identity;
            var temp = value;

            while (true)
            {
                if ((exponent & 1) != 0)
                    identity *= temp;

                exponent /= 2;

                if (exponent > 0)
                    temp *= temp;
                else
                    break;
            }

            result = identity;
        }

        public static Matrix3x3 Exponent(in Matrix3x3 value, int exponent)
        {
            Exponent(value, exponent, out var result);
            return result;
        }

        public static void Negate(in Matrix3x3 value, out Matrix3x3 result)
        {
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
        }

        public static Matrix3x3 Negate(in Matrix3x3 value)
        {
            Negate(value, out var result);
            return result;
        }

        public static void Lerp(in Matrix3x3 start, in Matrix3x3 end, float amount, out Matrix3x3 result)
        {
            result.M11 = MathUtil.Lerp(start.M11, end.M11, amount);
            result.M12 = MathUtil.Lerp(start.M12, end.M12, amount);
            result.M13 = MathUtil.Lerp(start.M13, end.M13, amount);
            result.M21 = MathUtil.Lerp(start.M21, end.M21, amount);
            result.M22 = MathUtil.Lerp(start.M22, end.M22, amount);
            result.M23 = MathUtil.Lerp(start.M23, end.M23, amount);
            result.M31 = MathUtil.Lerp(start.M31, end.M31, amount);
            result.M32 = MathUtil.Lerp(start.M32, end.M32, amount);
            result.M33 = MathUtil.Lerp(start.M33, end.M33, amount);
        }

        public static Matrix3x3 Lerp(in Matrix3x3 start, in Matrix3x3 end, float amount)
        {
            Lerp(start, end, amount, out var result);
            return result;
        }

        public static void SmoothStep(in Matrix3x3 start, in Matrix3x3 end, float amount, out Matrix3x3 result)
        {
            amount = MathUtil.SmoothStep(amount);
            Lerp(start, end, amount, out result);
        }

        public static Matrix3x3 SmoothStep(in Matrix3x3 start, in Matrix3x3 end, float amount)
        {
            SmoothStep(start, end, amount, out var result);
            return result;
        }

        public static void Transpose(in Matrix3x3 value, out Matrix3x3 result)
        {
            var temp = new Matrix3x3
            {
                M11 = value.M11,
                M12 = value.M21,
                M13 = value.M31,
                M21 = value.M12,
                M22 = value.M22,
                M23 = value.M32,
                M31 = value.M13,
                M32 = value.M23,
                M33 = value.M33
            };

            result = temp;
        }

        public static void TransposeByRef(in Matrix3x3 value, ref Matrix3x3 result)
        {
            result.M11 = value.M11;
            result.M12 = value.M21;
            result.M13 = value.M31;
            result.M21 = value.M12;
            result.M22 = value.M22;
            result.M23 = value.M32;
            result.M31 = value.M13;
            result.M32 = value.M23;
            result.M33 = value.M33;
        }

        public static Matrix3x3 Transpose(in Matrix3x3 value)
        {
            Transpose(value, out var result);
            return result;
        }

        public static void Invert(in Matrix3x3 value, out Matrix3x3 result)
        {
            var d11 = value.M22 * value.M33 + value.M23 * -value.M32;
            var d12 = value.M21 * value.M33 + value.M23 * -value.M31;
            var d13 = value.M21 * value.M32 + value.M22 * -value.M31;

            var det = value.M11 * d11 - value.M12 * d12 + value.M13 * d13;
            if (Math.Abs(det) == 0)
            {
                result = Matrix3x3.Zero;
                return;
            }

            det = 1 / det;

            var d21 = value.M12 * value.M33 + value.M13 * -value.M32;
            var d22 = value.M11 * value.M33 + value.M13 * -value.M31;
            var d23 = value.M11 * value.M32 + value.M12 * -value.M31;

            var d31 = (value.M12 * value.M23) - (value.M13 * value.M22);
            var d32 = (value.M11 * value.M23) - (value.M13 * value.M21);
            var d33 = (value.M11 * value.M22) - (value.M12 * value.M21);

            result.M11 = +d11 * det; result.M12 = -d21 * det; result.M13 = +d31 * det;
            result.M21 = -d12 * det; result.M22 = +d22 * det; result.M23 = -d32 * det;
            result.M31 = +d13 * det; result.M32 = -d23 * det; result.M33 = +d33 * det;
        }

        public static Matrix3x3 Invert(in Matrix3x3 value)
        {
            value.Invert();
            return value;
        }

        public static void Orthogonalize(in Matrix3x3 value, out Matrix3x3 result)
        {
            result = value;

            result.Row2 -= (Vector3.Dot(result.Row1, result.Row2) / Vector3.Dot(result.Row1, result.Row1)) * result.Row1;

            result.Row3 -= (Vector3.Dot(result.Row1, result.Row3) / Vector3.Dot(result.Row1, result.Row1)) * result.Row1;
            result.Row3 -= (Vector3.Dot(result.Row2, result.Row3) / Vector3.Dot(result.Row2, result.Row2)) * result.Row2;
        }

        public static Matrix3x3 Orthogonalize(in Matrix3x3 value)
        {
            Orthogonalize(value, out var result);
            return result;
        }

        public static void Orthonormalize(in Matrix3x3 value, out Matrix3x3 result)
        {
            result = value;

            result.Row1 = Vector3.Normalize(result.Row1);

            result.Row2 -= Vector3.Dot(result.Row1, result.Row2) * result.Row1;
            result.Row2 = Vector3.Normalize(result.Row2);

            result.Row3 -= Vector3.Dot(result.Row1, result.Row3) * result.Row1;
            result.Row3 -= Vector3.Dot(result.Row2, result.Row3) * result.Row2;
            result.Row3 = Vector3.Normalize(result.Row3);
        }

        public static Matrix3x3 Orthonormalize(in Matrix3x3 value)
        {
            Orthonormalize(value, out var result);
            return result;
        }

        public static void UpperTriangularForm(in Matrix3x3 value, out Matrix3x3 result)
        {
            result = value;
            var lead = 0;
            var rowcount = 3;
            var columncount = 3;

            for (var r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                var i = r;

                while (MathUtil.NearZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                var multiplier = 1 / result[r, lead];

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
                        result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
                        result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
                    }
                }

                lead++;
            }
        }

        public static Matrix3x3 UpperTriangularForm(in Matrix3x3 value)
        {
            UpperTriangularForm(value, out var result);
            return result;
        }

        public static void LowerTriangularForm(in Matrix3x3 value, out Matrix3x3 result)
        {
            var temp = value;
            Transpose(temp, out result);

            var lead = 0;
            var rowcount = 3;
            var columncount = 3;

            for (var r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                var i = r;

                while (MathUtil.NearZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                var multiplier = 1 / result[r, lead];

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
                        result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
                        result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
                    }
                }

                lead++;
            }

            Transpose(result, out result);
        }

        public static Matrix3x3 LowerTriangularForm(in Matrix3x3 value)
        {
            LowerTriangularForm(value, out var result);
            return result;
        }

        public static void RowEchelonForm(in Matrix3x3 value, out Matrix3x3 result)
        {

            result = value;
            var lead = 0;
            var rowcount = 3;
            var columncount = 3;

            for (var r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                var i = r;

                while (MathUtil.NearZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                var multiplier = 1 / result[r, lead];
                result[r, 0] *= multiplier;
                result[r, 1] *= multiplier;
                result[r, 2] *= multiplier;

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * result[i, lead];
                        result[i, 1] -= result[r, 1] * result[i, lead];
                        result[i, 2] -= result[r, 2] * result[i, lead];
                    }
                }

                lead++;
            }
        }

        public static Matrix3x3 RowEchelonForm(in Matrix3x3 value)
        {
            RowEchelonForm(value, out var result);
            return result;
        }

        public static void BillboardLH(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector, out Matrix3x3 result)
        {
            var difference = cameraPosition - objectPosition;

            var lengthSq = difference.LengthSquared();
            if (MathUtil.NearZero(lengthSq)) difference = -cameraForwardVector;
            else difference *= (float)(1.0 / Math.Sqrt(lengthSq));

            var crossed = Vector3.Normalize(Vector3.Cross(cameraUpVector, difference));
            var final = Vector3.Cross(difference, crossed);

            result.M11 = crossed.X;
            result.M12 = crossed.Y;
            result.M13 = crossed.Z;
            result.M21 = final.X;
            result.M22 = final.Y;
            result.M23 = final.Z;
            result.M31 = difference.X;
            result.M32 = difference.Y;
            result.M33 = difference.Z;
        }

        public static Matrix3x3 BillboardLH(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector)
        {
            BillboardLH(objectPosition, cameraPosition, cameraUpVector, cameraForwardVector, out var result);
            return result;
        }

        public static void BillboardRH(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector, out Matrix3x3 result)
        {
            var difference = objectPosition - cameraPosition;

            var lengthSq = difference.LengthSquared();
            if (MathUtil.NearZero(lengthSq)) difference = -cameraForwardVector;
            else difference *= (float)(1.0 / Math.Sqrt(lengthSq));

            var crossed = Vector3.Normalize(Vector3.Cross(cameraUpVector, difference));
            var final = Vector3.Cross(difference, crossed);

            result.M11 = crossed.X;
            result.M12 = crossed.Y;
            result.M13 = crossed.Z;
            result.M21 = final.X;
            result.M22 = final.Y;
            result.M23 = final.Z;
            result.M31 = difference.X;
            result.M32 = difference.Y;
            result.M33 = difference.Z;
        }

        public static Matrix3x3 BillboardRH(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector)
        {
            BillboardRH(objectPosition, cameraPosition, cameraUpVector, cameraForwardVector, out var result);
            return result;
        }

        public static void LookAtLH(in Vector3 eye, in Vector3 target, in Vector3 up, out Matrix3x3 result)
        {
            var zaxis = Vector3.Normalize(target - eye);
            var xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            var yaxis = Vector3.Cross(zaxis, xaxis);

            result = Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;
        }

        public static Matrix3x3 LookAtLH(in Vector3 eye, in Vector3 target, in Vector3 up)
        {
            LookAtLH(eye, target, up, out var result);
            return result;
        }

        public static void LookAtRH(in Vector3 eye, in Vector3 target, in Vector3 up, out Matrix3x3 result)
        {
            var zaxis = Vector3.Normalize(eye - target);
            var xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            var yaxis = Vector3.Cross(zaxis, xaxis);

            result = Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;
        }

        public static Matrix3x3 LookAtRH(in Vector3 eye, in Vector3 target, in Vector3 up)
        {
            LookAtRH(eye, target, up, out var result);
            return result;
        }

        public static void Scaling(in Vector3 scale, out Matrix3x3 result)
        {
            Scaling(scale.X, scale.Y, scale.Z, out result);
        }

        public static Matrix3x3 Scaling(in Vector3 scale)
        {
            Scaling(scale, out var result);
            return result;
        }

        public static void Scaling(float x, float y, float z, out Matrix3x3 result)
        {
            result = Identity;
            result.M11 = x;
            result.M22 = y;
            result.M33 = z;
        }

        public static Matrix3x3 Scaling(float x, float y, float z)
        {
            Scaling(x, y, z, out var result);
            return result;
        }

        public static void Scaling(float scale, out Matrix3x3 result)
        {
            result = Identity;
            result.M11 = result.M22 = result.M33 = scale;
        }

        public static Matrix3x3 Scaling(float scale)
        {
            Scaling(scale, out var result);
            return result;
        }

        public static void RotationX(float angle, out Matrix3x3 result)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            result = Identity;
            result.M22 = cos;
            result.M23 = sin;
            result.M32 = -sin;
            result.M33 = cos;
        }

        public static Matrix3x3 RotationX(float angle)
        {
            RotationX(angle, out var result);
            return result;
        }

        public static void RotationY(float angle, out Matrix3x3 result)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            result = Identity;
            result.M11 = cos;
            result.M13 = -sin;
            result.M31 = sin;
            result.M33 = cos;
        }

        public static Matrix3x3 RotationY(float angle)
        {
            RotationY(angle, out var result);
            return result;
        }

        public static void RotationZ(float angle, out Matrix3x3 result)
        {
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);

            result = Identity;
            result.M11 = cos;
            result.M12 = sin;
            result.M21 = -sin;
            result.M22 = cos;
        }

        public static Matrix3x3 RotationZ(float angle)
        {
            RotationZ(angle, out var result);
            return result;
        }

        public static void RotationAxis(in Vector3 axis, float angle, out Matrix3x3 result)
        {
            var x = axis.X;
            var y = axis.Y;
            var z = axis.Z;
            var cos = (float)Math.Cos(angle);
            var sin = (float)Math.Sin(angle);
            var xx = x * x;
            var yy = y * y;
            var zz = z * z;
            var xy = x * y;
            var xz = x * z;
            var yz = y * z;

            result = Identity;
            result.M11 = xx + (cos * (1 - xx));
            result.M12 = (xy - (cos * xy)) + (sin * z);
            result.M13 = (xz - (cos * xz)) - (sin * y);
            result.M21 = (xy - (cos * xy)) - (sin * z);
            result.M22 = yy + (cos * (1 - yy));
            result.M23 = (yz - (cos * yz)) + (sin * x);
            result.M31 = (xz - (cos * xz)) + (sin * y);
            result.M32 = (yz - (cos * yz)) - (sin * x);
            result.M33 = zz + (cos * (1 - zz));
        }

        public static Matrix3x3 RotationAxis(in Vector3 axis, float angle)
        {
            RotationAxis(axis, angle, out var result);
            return result;
        }

        public static void RotationQuaternion(in Quaternion rotation, out Matrix3x3 result)
        {
            var xx = rotation.X * rotation.X;
            var yy = rotation.Y * rotation.Y;
            var zz = rotation.Z * rotation.Z;
            var xy = rotation.X * rotation.Y;
            var zw = rotation.Z * rotation.W;
            var zx = rotation.Z * rotation.X;
            var yw = rotation.Y * rotation.W;
            var yz = rotation.Y * rotation.Z;
            var xw = rotation.X * rotation.W;

            result = Identity;
            result.M11 = 1 - (2 * (yy + zz));
            result.M12 = 2 * (xy + zw);
            result.M13 = 2 * (zx - yw);
            result.M21 = 2 * (xy - zw);
            result.M22 = 1 - (2 * (zz + xx));
            result.M23 = 2 * (yz + xw);
            result.M31 = 2 * (zx + yw);
            result.M32 = 2 * (yz - xw);
            result.M33 = 1 - (2 * (yy + xx));
        }

        public static Matrix3x3 CreateFromQuaternion(in Quaternion rotation)
        {
            RotationQuaternion(rotation, out var result);
            return result;
        }

        public static void RotationYawPitchRoll(float yaw, float pitch, float roll, out Matrix3x3 result)
        {
            var quaternion = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            RotationQuaternion(quaternion, out result);
        }

        public static Matrix3x3 RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            RotationYawPitchRoll(yaw, pitch, roll, out var result);
            return result;
        }

        public static Matrix3x3 operator +(in Matrix3x3 left, in Matrix3x3 right)
        {
            Add(left, right, out var result);
            return result;
        }

        public static Matrix3x3 operator +(in Matrix3x3 value)
        {
            return value;
        }

        public static Matrix3x3 operator -(in Matrix3x3 left, in Matrix3x3 right)
        {
            Subtract(left, right, out var result);
            return result;
        }

        public static Matrix3x3 operator -(in Matrix3x3 value)
        {
            Negate(value, out var result);
            return result;
        }

        public static Matrix3x3 operator *(float left, in Matrix3x3 right)
        {
            Multiply(right, left, out var result);
            return result;
        }

        public static Matrix3x3 operator *(in Matrix3x3 left, float right)
        {
            Multiply(left, right, out var result);
            return result;
        }

        public static Matrix3x3 operator *(in Matrix3x3 left, in Matrix3x3 right)
        {
            Multiply(left, right, out var result);
            return result;
        }

        public static Matrix3x3 operator /(in Matrix3x3 left, float right)
        {
            Divide(left, right, out var result);
            return result;
        }

        public static Matrix3x3 operator /(in Matrix3x3 left, in Matrix3x3 right)
        {
            Divide(left, right, out var result);
            return result;
        }

        public static bool operator ==(in Matrix3x3 left, in Matrix3x3 right) => left.Equals(right);
        public static bool operator !=(in Matrix3x3 left, in Matrix3x3 right) => !left.Equals(right);

        public static explicit operator Matrix4x4(in Matrix3x3 value)
        {
            return new Matrix4x4(
                value.M11, value.M12, value.M13, 0,
                value.M21, value.M22, value.M23, 0,
                value.M31, value.M32, value.M33, 0,
                0, 0, 0, 1
                );
        }

        public override string ToString() => $"{{ {{M11:{M11} M12:{M12} M13:{M13}}} {{M21:{M21} M22:{M22} M23:{M23}}} {{M31:{M31} M32:{M32} M33:{M33}}} }}";

        public override int GetHashCode()
        {
            var hash = HashCode.Initializer;
            hash.Combine(M11.GetHashCode());
            hash.Combine(M12.GetHashCode());
            hash.Combine(M13.GetHashCode());
            hash.Combine(M21.GetHashCode());
            hash.Combine(M22.GetHashCode());
            hash.Combine(M23.GetHashCode());
            hash.Combine(M31.GetHashCode());
            hash.Combine(M32.GetHashCode());
            hash.Combine(M33.GetHashCode());
            return hash;
        }

        public bool Equals(Matrix3x3 other)
        {
            return 
                other.M11 == M11 &&
                other.M12 == M12 &&
                other.M13 == M13 &&
                other.M21 == M21 &&
                other.M22 == M22 &&
                other.M23 == M23 &&
                other.M31 == M31 &&
                other.M32 == M32 &&
                other.M33 == M33;
        }

        public override bool Equals(object obj) => obj is Matrix3x3 other && Equals(other);
    }
}
