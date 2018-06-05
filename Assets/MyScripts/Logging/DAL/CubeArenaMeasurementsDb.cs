using System;
using System.Linq.Expressions;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;

namespace CubeArena.Assets.MyScripts.Logging.DAL {
    interface CubeArenaMeasurementsDb {

        int GetNextPlayerId ();
        T Find<T> (Expression<Func<T, bool>> condition) where T : BaseEntity, new ();
        Assist InsertAssist (Assist assist);
        PlayerRound InsertPlayerRound (PlayerRound playerRound);
        Placement InsertPlacement (Placement placement);
        Selection InsertSelection (Selection selection);
        SelectionAction InsertSelectionAction (SelectionAction selectionAction);
        Kill InsertKill (Kill kill);
        Move InsertMove (Move move);
        Rotation InsertRotation (Rotation rotation);
        AreaInteraction InsertAreaInteraction (AreaInteraction areaInteraction);
        CloudMeasurement InsertCloudMeasurement (CloudMeasurement cloudMeasurement);
        Device GetDeviceByModel (string model);
        Device InsertDevice (Device device);
        RatingAnswer SaveRatingAnswer(RatingAnswer answer);
        WeightAnswer SaveWeightAnswer(WeightAnswer arg);
    }
}