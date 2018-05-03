using System;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
    public class InvalidTransformDirectionException : ApplicationException {
        public InvalidTransformDirectionException (TransformDirection direction) :
             base (string.Format ("\"{0}\" is not a valid TransformDirection", direction)) { }
    }
}