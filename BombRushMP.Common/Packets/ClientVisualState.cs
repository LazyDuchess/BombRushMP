using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientVisualState : Packet
    {
        public override Packets PacketId => Packets.ClientVisualState;
        public Vector3 Position = Vector3.Zero;
        public Vector3 VisualPosition = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Quaternion VisualRotation = Quaternion.Identity;
        public bool MoveStyleEquipped = false;
        public int MoveStyle = 0;
        public float GrindDirection = 0f;
        public bool SprayCanHeld = false;
        public bool PhoneHeld = false;
        public float PhoneDirectionX = 0f;
        public float PhoneDirectionY = 0f;
        public float TurnDirection1 = 0f;
        public float TurnDirection2 = 0f;
        public float TurnDirection3 = 0f;
        public float TurnDirectionSkateboard = 0f;
        public PlayerStates State = PlayerStates.None;
        public int DustEmissionRate = 0;
        public int BoostpackEffectMode = 0;
        public int FrictionEffectMode = 0;
        public int CurrentAnimation = 0;
        public float CurrentAnimationTime = 0f;
        public bool AFK = false;
        public int MoveStyleSkin = 0;
        public int MPMoveStyleSkin = -1;
        public int HitBoxMask = 0;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)State);

            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);

            writer.Write(VisualPosition.X);
            writer.Write(VisualPosition.Y);
            writer.Write(VisualPosition.Z);

            Compression.WriteCompressedQuaternion(Rotation, writer);

            Compression.WriteCompressedQuaternion(VisualRotation, writer);

            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
            writer.Write(Velocity.Z);

            writer.Write(MoveStyleEquipped);
            writer.Write(MoveStyle);

            writer.Write(SprayCanHeld);
            writer.Write(PhoneHeld);

            writer.Write(PhoneDirectionX);
            writer.Write(PhoneDirectionY);

            writer.Write(TurnDirection1);
            writer.Write(TurnDirection2);
            writer.Write(TurnDirection3);
            writer.Write(TurnDirectionSkateboard);

            writer.Write(GrindDirection);

            writer.Write(DustEmissionRate);
            writer.Write(BoostpackEffectMode);
            writer.Write(FrictionEffectMode);

            writer.Write(CurrentAnimation);
            writer.Write(CurrentAnimationTime);

            writer.Write(AFK);

            writer.Write(MoveStyleSkin);
            writer.Write(MPMoveStyleSkin);

            writer.Write(HitBoxMask);
        }

        public override void Read(BinaryReader reader)
        {
            State = (PlayerStates)reader.ReadInt32();

            var posX = reader.ReadSingle();
            var posY = reader.ReadSingle();
            var posZ = reader.ReadSingle();

            var visualPosX = reader.ReadSingle();
            var visualPosY = reader.ReadSingle();
            var visualPosZ = reader.ReadSingle();

            Rotation = Compression.ReadCompressedQuaternion(reader);

            VisualRotation = Compression.ReadCompressedQuaternion(reader);

            var velX = reader.ReadSingle();
            var velY = reader.ReadSingle();
            var velZ = reader.ReadSingle();

            MoveStyleEquipped = reader.ReadBoolean();
            MoveStyle = reader.ReadInt32();

            SprayCanHeld = reader.ReadBoolean();
            PhoneHeld = reader.ReadBoolean();

            PhoneDirectionX = reader.ReadSingle();
            PhoneDirectionY = reader.ReadSingle();

            TurnDirection1 = reader.ReadSingle();
            TurnDirection2 = reader.ReadSingle();
            TurnDirection3 = reader.ReadSingle();
            TurnDirectionSkateboard = reader.ReadSingle();

            GrindDirection = reader.ReadSingle();

            Position = new Vector3(posX, posY, posZ);
            VisualPosition = new Vector3(visualPosX, visualPosY, visualPosZ);
            Velocity = new Vector3(velX, velY, velZ);

            DustEmissionRate = reader.ReadInt32();
            BoostpackEffectMode = reader.ReadInt32();
            FrictionEffectMode = reader.ReadInt32();

            CurrentAnimation = reader.ReadInt32();
            CurrentAnimationTime = reader.ReadSingle();

            AFK = reader.ReadBoolean();

            MoveStyleSkin = reader.ReadInt32();
            MPMoveStyleSkin = reader.ReadInt32();

            HitBoxMask = reader.ReadInt32();
        }
    }
}
