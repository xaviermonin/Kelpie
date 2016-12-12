using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity.Construction.Helpers.Emitter
{
    /// <summary>
    /// A wrapper chainable ilGenerator.
    /// <seealso cref="System.Reflection.Emit.ILGenerator"/>
    /// Freely inspired by Fasterflect.
    /// </summary>
    class EmitHelper
    {
        /// <summary>
        /// EmitHelper constructor
        /// </summary>
        /// <param name="ilGenerator">IGenerator</param>
        public EmitHelper(ILGenerator ilGenerator)
        {
            if (ilGenerator == null)
                throw new ArgumentNullException("ilGenerator");

            ILGenerator = ilGenerator;
        }

        public ILGenerator ILGenerator
        {
            get; private set;
        }

        /// <summary>
        /// Converts the supplied <see cref="EmitHelper"/> to a <see cref="ILGenerator"/>.
        /// </summary>
        /// <param name="emitHelper">The <see cref="EmitHelper"/>.</param>
        /// <returns>An ILGenerator.</returns>
        public static implicit operator ILGenerator(EmitHelper emitHelper)
        {
            if (emitHelper == null)
                throw new ArgumentNullException("emitHelper");

            return emitHelper.ILGenerator;
        }

        #region ILGenerator Methods

        /// <summary>
        /// Declares a local variable.
        /// </summary>
        /// <param name="localType">The Type of the local variable.</param>
        /// <returns>The declared local variable.</returns>
        public LocalBuilder DeclareLocal(Type localType)
        {
            return ILGenerator.DeclareLocal(localType);
        }

        /// <summary>
	    /// Declares a new label.
	    /// </summary>
	    /// <returns>Returns a new label that can be used as a token for branching.</returns>
	    public Label DefineLabel()
        {
            return ILGenerator.DefineLabel();
        }

        /// <summary>
	    /// Marks the Microsoft intermediate language (MSIL) stream's current position 
	    /// with the given label.
	    /// </summary>
	    /// <param name="loc">The label for which to set an index.</param>
	    /// <returns>Current instance of the <see cref="EmitHelper"/>.</returns>
	    public EmitHelper MarkLabel(Label loc)
        {
            ILGenerator.MarkLabel(loc);
            return this;
        }

        #endregion

        #region Emit Wrappers

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Call"/>, <see cref="MethodInfo"/>) or 
        /// ILGenerator.Emit(<see cref="OpCodes.Callvirt"/>, <see cref="MethodInfo"/>) that
        /// call the method specified
        /// </summary>
        /// <param name="methodInfo">MethodInfo that describe the method to call</param>
        /// <returns></returns>
        public EmitHelper call(MethodInfo methodInfo)
        {
            ILGenerator.Emit(methodInfo.IsStatic ? OpCodes.Call : OpCodes.Callvirt, methodInfo);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldloc"/>, short) that
        /// load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the local variable value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldloc">OpCodes.Ldloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public EmitHelper ldloc(short index)
        {
            ILGenerator.Emit(OpCodes.Ldloc, index);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Nop"/>) that
        /// fills space if opcodes are patched. No meaningful operation is performed although 
        /// a processing cycle can be consumed.
        /// </summary>
        /// <seealso cref="OpCodes.Nop">OpCodes.Nop</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitHelper nop()
        {
            ILGenerator.Emit(OpCodes.Nop);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Br_S"/>, label) that
	    /// unconditionally transfers control to a target instruction (short form).
	    /// </summary>
	    /// <param name="label">The label to branch from this location.</param>
	    /// <seealso cref="OpCodes.Br_S">OpCodes.Br_S</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
	    public EmitHelper br_s(Label label)
        {
            ILGenerator.Emit(OpCodes.Br_S, label);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short) or 
	    /// ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
	    /// loads an argument (referenced by a specified index value) onto the stack.
	    /// </summary>
	    /// <param name="index">Index of the argument that is pushed onto the stack.</param>
	    /// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
	    public EmitHelper ldarg(int index)
        {
            switch (index)
            {
                case 0:
                    ldarg_0();
                    break;
                case 1:
                    ldarg_1();
                    break;
                case 2:
                    ldarg_2();
                    break;
                case 3:
                    ldarg_3();
                    break;
                default:
                    if (index <= byte.MaxValue)
                    {
                        ldarg_s((byte)index);
                    }
                    else if (index <= short.MaxValue)
                    {
                        ldarg((short)index);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    break;
            }
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue"/>, label) that
	    /// transfers control to a target instruction if value is true, not null, or non-zero.
	    /// </summary>
	    /// <param name="label">The label to branch from this location.</param>
	    /// <seealso cref="OpCodes.Brtrue">OpCodes.Brtrue</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
	    public EmitHelper brtrue(Label label)
        {
            ILGenerator.Emit(OpCodes.Brtrue, label);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Brtrue_S"/>, label) that
	    /// transfers control to a target instruction (short form) if value is true, not null, or non-zero.
	    /// </summary>
	    /// <param name="label">The label to branch from this location.</param>
	    /// <seealso cref="OpCodes.Brtrue_S">OpCodes.Brtrue_S</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
	    public EmitHelper brtrue_s(Label label)
        {
            ILGenerator.Emit(OpCodes.Brtrue_S, label);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Newobj"/>, <see cref="ConstructorInfo"/>) that
	    /// creates a new object or a new instance of a value type,
	    /// pushing an object reference (type O) onto the evaluation stack.
	    /// </summary>
	    /// <param name="constructorInfo">A <see cref="ConstructorInfo"/> representing a constructor.</param>
	    /// <seealso cref="OpCodes.Newobj">OpCodes.Newobj</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,ConstructorInfo)">ILGenerator.Emit</seealso>
	    public EmitHelper newobj(ConstructorInfo constructorInfo)
        {
            ILGenerator.Emit(OpCodes.Newobj, constructorInfo);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_0"/>) that
	    /// loads the argument at index 0 onto the evaluation stack.
	    /// </summary>
	    /// <seealso cref="OpCodes.Ldarg_0">OpCodes.Ldarg_0</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
	    public EmitHelper ldarg_0()
        {
            ILGenerator.Emit(OpCodes.Ldarg_0);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_1"/>) that
        /// loads the argument at index 1 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_1">OpCodes.Ldarg_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitHelper ldarg_1()
        {
            ILGenerator.Emit(OpCodes.Ldarg_1);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Isinst"/>) that
        /// Tests whether an object reference (type O) is an instance of a particular class.
        /// </summary>
        /// <param name="type">Tested Type</param>
        public EmitHelper isinst(Type type)
        {
            ILGenerator.Emit(OpCodes.Isinst, type);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_2"/>) that
        /// loads the argument at index 2 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_2">OpCodes.Ldarg_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitHelper ldarg_2()
        {
            ILGenerator.Emit(OpCodes.Ldarg_0);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_3"/>) that
        /// loads the argument at index 3 onto the evaluation stack.
        /// </summary>
        /// <seealso cref="OpCodes.Ldarg_3">OpCodes.Ldarg_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public EmitHelper ldarg_3()
        {
            ILGenerator.Emit(OpCodes.Ldarg_0);
            return this;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
        /// loads the argument (referenced by a specified short form index) onto the evaluation stack.
        /// </summary>
        /// <param name="index">Index of the argument value that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg_S">OpCodes.Ldarg_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public EmitHelper ldarg_s(byte index)
        {
            ILGenerator.Emit(OpCodes.Ldarg_S, index);
            return this;
        }

        /// <summary>
        /// Push the value of a field onto the evaluation stack.
        /// Call ILGenerator.Emit(<see cref="OpCodes.Ldfld"/>) or
        /// ILGenerator.Emit(<see cref="OpCodes.Ldsfld"/>).
        /// </summary>
        /// <param name="isStatic">true if the field is static</param>
        /// <param name="fieldInfo">Field to push onto evaluation stack</param>
        /// <returns></returns>
        public EmitHelper ldfld(FieldInfo fieldInfo, bool isStatic = false)
        {
            ILGenerator.Emit(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fieldInfo);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Stfld"/>, <see cref="FieldInfo"/>) that
	    /// replaces the value stored in the field of an object reference or pointer with a new value.
	    /// </summary>
	    /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
	    /// <seealso cref="OpCodes.Stfld">OpCodes.Stfld</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
	    public EmitHelper stfld(FieldInfo fieldInfo)
        {
            ILGenerator.Emit(OpCodes.Stfld, fieldInfo);
            return this;
        }

        /// <summary>
	    /// Calls ILGenerator.Emit(<see cref="OpCodes.Ret"/>) that
	    /// returns from the current method, pushing a return value (if present) 
	    /// from the caller's evaluation stack onto the callee's evaluation stack.
	    /// </summary>
	    /// <seealso cref="OpCodes.Ret">OpCodes.Ret</seealso>
	    /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
	    public EmitHelper ret()
        {
            ILGenerator.Emit(OpCodes.Ret);
            return this;
        }

        #endregion
}
}
