static public class MarioTable
{
	public const int speedRun = 6;
	public const float speedLongJump = 11.39f;
	public const float speedSquat = 3f;
	public static readonly float[][] dataJump = new float[][] {
		// { impulse,   minHeight,   maxHeight }
		new float[] { 10f,       1.5f,        3f    },  // Jump 1
		new float[] { 10f,       2.0f,         3.5f    },  // Jump 2
		new float[] { 10f,      3.2f,         5f    }   // Jump 3
	};

    //public const int maxMoveJump = 3;
    public const int speedTurnWalk = 30;
    public const int speedTurnJump = 4;
    public const int speedTurnFall = 7;
    public const int speedTurnSquat = 5;
};