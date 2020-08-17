using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BimExperts.Model
{
    class MagicRenumberLogic
    {
        public ICollection<ElementId> elementsForRenumbering = null;
        public ICollection<ElementId> renumberingOrigin = null;
        public List<string> parameterNames = null;

        public void renumberElements()
        {

        }


    }
}
