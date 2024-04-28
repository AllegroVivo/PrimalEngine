using System;
using System.Numerics;
using System.Runtime.Serialization;
using Editor.Utilities;

namespace Editor.Components;

[DataContract]
class Transform : Component
{
    private Vector3 _position;

    [DataMember]
    public Vector3 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
    }
    
    private Vector3 _rotation;
    [DataMember]
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                OnPropertyChanged(nameof(Rotation));
            }
        }
    }

    private Vector3 _scale;
    [DataMember]
    public Vector3 Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
    }

    public Transform(GameEntity owner) : base(owner)
    { }

    public override IMSComponent GetMultiselectionComponent(MSEntity msEntity) => new MSTransform(msEntity);
}

sealed class MSTransform : MSComponent<Transform>
{
    private Single? _posX;
    public Single? PosX
    {
        get => _posX;
        set
        {
            if (!_posX.IsTheSameAs(value))
            {
                _posX = value;
                OnPropertyChanged(nameof(PosX));
            }
        }
    }

    private Single? _posY;
    public Single? PosY
    {
        get => _posY;
        set
        {
            if (!_posY.IsTheSameAs(value))
            {
                _posY = value;
                OnPropertyChanged(nameof(PosY));
            }
        }
    }

    private Single? _posZ;
    public Single? PosZ
    {
        get => _posZ;
        set
        {
            if (!_posZ.IsTheSameAs(value))
            {
                _posZ = value;
                OnPropertyChanged(nameof(PosZ));
            }
        }
    }
    
    private Single? _rotX;
    public Single? RotX
    {
        get => _rotX;
        set
        {
            if (!_rotX.IsTheSameAs(value))
            {
                _rotX = value;
                OnPropertyChanged(nameof(RotX));
            }
        }
    }

    private Single? _rotY;
    public Single? RotY
    {
        get => _rotY;
        set
        {
            if (!_rotY.IsTheSameAs(value))
            {
                _rotY = value;
                OnPropertyChanged(nameof(RotY));
            }
        }
    }

    private Single? _rotZ;
    public Single? RotZ
    {
        get => _rotZ;
        set
        {
            if (!_rotZ.IsTheSameAs(value))
            {
                _rotZ = value;
                OnPropertyChanged(nameof(RotZ));
            }
        }
    }
    
    private Single? _scaleX;
    public Single? ScaleX
    {
        get => _scaleX;
        set
        {
            if (!_scaleX.IsTheSameAs(value))
            {
                _scaleX = value;
                OnPropertyChanged(nameof(ScaleX));
            }
        }
    }

    private Single? _scaleY;
    public Single? ScaleY
    {
        get => _scaleY;
        set
        {
            if (!_scaleY.IsTheSameAs(value))
            {
                _scaleY = value;
                OnPropertyChanged(nameof(ScaleY));
            }
        }
    }

    private Single? _scaleZ;
    public Single? ScaleZ
    {
        get => _scaleZ;
        set
        {
            if (!_scaleZ.IsTheSameAs(value))
            {
                _scaleZ = value;
                OnPropertyChanged(nameof(ScaleZ));
            }
        }
    }
    
    public MSTransform(MSEntity msEntity) : base(msEntity)
    {
        Refresh();
    }

    protected override Boolean UpdateComponents(String propertyName)
    {
        switch (propertyName)
        {
            case nameof(PosX):
            case nameof(PosY):
            case nameof(PosZ):
                SelectedComponents.ForEach(c => c.Position = new Vector3(_posX ?? c.Position.X, _posY ?? c.Position.Y, _posZ ?? c.Position.Z));
                return true;
                
            case nameof(RotX):
            case nameof(RotY):
            case nameof(RotZ):
                SelectedComponents.ForEach(c => c.Rotation = new Vector3(_rotX ?? c.Rotation.X, _rotY ?? c.Rotation.Y, _rotZ ?? c.Rotation.Z));
                return true;
                
            case nameof(ScaleX):
            case nameof(ScaleY):
            case nameof(ScaleZ):
                SelectedComponents.ForEach(c => c.Scale = new Vector3(_scaleX ?? c.Scale.X, _scaleY ?? c.Scale.Y, _scaleZ ?? c.Scale.Z));
                return true;
        }

        return false;
    }

    protected override Boolean UpdateMSComponent()
    {
        PosX = MSEntity.GetMixedValue(SelectedComponents, x => x.Position.X);
        PosY = MSEntity.GetMixedValue(SelectedComponents, x => x.Position.Y);
        PosZ = MSEntity.GetMixedValue(SelectedComponents, x => x.Position.Z);
        
        RotX = MSEntity.GetMixedValue(SelectedComponents, x => x.Rotation.X);
        RotY = MSEntity.GetMixedValue(SelectedComponents, x => x.Rotation.Y);
        RotZ = MSEntity.GetMixedValue(SelectedComponents, x => x.Rotation.Z);
        
        ScaleX = MSEntity.GetMixedValue(SelectedComponents, x => x.Scale.X);
        ScaleY = MSEntity.GetMixedValue(SelectedComponents, x => x.Scale.Y);
        ScaleZ = MSEntity.GetMixedValue(SelectedComponents, x => x.Scale.Z);

        return true;
    }
}