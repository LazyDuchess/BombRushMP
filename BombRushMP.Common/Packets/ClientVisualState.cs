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
        private enum BooleanMask
        {
            MoveStyleEquipped,
            SprayCanHeld,
            PhoneHeld,
            AFK,
            Hitbox,
            HitboxLeftLeg,
            HitboxRightLeg,
            HitboxUpperBody,
            HitboxAerial,
            HitboxRadial,
            HitboxSpray
        }
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
        public byte DustEmissionRate = 0;
        public byte BoostpackEffectMode = 0;
        public byte FrictionEffectMode = 0;
        public int CurrentAnimation = 0;
        public float CurrentAnimationTime = 0f;
        public bool AFK = false;
        public byte MoveStyleSkin = 0;
        public int MPMoveStyleSkin = -1;
        public bool Hitbox = false;
        public bool HitboxLeftLeg = false;
        public bool HitboxRightLeg = false;
        public bool HitboxUpperBody = false;
        public bool HitboxAerial = false;
        public bool HitboxRadial = false;
        public bool HitboxSpray = false;

        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)State);

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

            var bitField = new Bitfield();
            bitField[BooleanMask.MoveStyleEquipped] = MoveStyleEquipped;
            bitField[BooleanMask.SprayCanHeld] = SprayCanHeld;
            bitField[BooleanMask.PhoneHeld] = PhoneHeld;
            bitField[BooleanMask.AFK] = AFK;
            bitField[BooleanMask.Hitbox] = Hitbox;
            bitField[BooleanMask.HitboxLeftLeg] = HitboxLeftLeg;
            bitField[BooleanMask.HitboxRightLeg] = HitboxRightLeg;
            bitField[BooleanMask.HitboxUpperBody] = HitboxUpperBody;
            bitField[BooleanMask.HitboxAerial] = HitboxAerial;
            bitField[BooleanMask.HitboxRadial] = HitboxRadial;
            bitField[BooleanMask.HitboxSpray] = HitboxSpray;

            bitField.WriteShort(writer);

            writer.Write(MoveStyle);

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

            writer.Write(MoveStyleSkin);
            writer.Write(MPMoveStyleSkin);
        }

        public override void Read(BinaryReader reader)
        {
            State = (PlayerStates)reader.ReadByte();

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

            var bitField = Bitfield.ReadShort(reader);
            MoveStyleEquipped = bitField[BooleanMask.MoveStyleEquipped];
            SprayCanHeld = bitField[BooleanMask.SprayCanHeld];
            PhoneHeld = bitField[BooleanMask.PhoneHeld];
            AFK = bitField[BooleanMask.AFK] = AFK;
            Hitbox = bitField[BooleanMask.Hitbox] = Hitbox;
            HitboxLeftLeg = bitField[BooleanMask.HitboxLeftLeg];
            HitboxRightLeg = bitField[BooleanMask.HitboxRightLeg];
            HitboxUpperBody = bitField[BooleanMask.HitboxUpperBody];
            HitboxAerial = bitField[BooleanMask.HitboxAerial];
            HitboxRadial = bitField[BooleanMask.HitboxRadial];
            HitboxSpray = bitField[BooleanMask.HitboxSpray];

            MoveStyle = reader.ReadInt32();

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

            DustEmissionRate = reader.ReadByte();
            BoostpackEffectMode = reader.ReadByte();
            FrictionEffectMode = reader.ReadByte();

            CurrentAnimation = reader.ReadInt32();
            CurrentAnimationTime = reader.ReadSingle();

            MoveStyleSkin = reader.ReadByte();
            MPMoveStyleSkin = reader.ReadInt32();
        }
    }
}
