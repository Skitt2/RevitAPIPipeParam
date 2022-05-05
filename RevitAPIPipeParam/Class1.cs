using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIPipeParam
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application application = uiapp.Application;

            List<Pipe> pipes = new FilteredElementCollector(doc)
            .OfClass(typeof(Pipe))
            .Cast<Pipe>()
            .ToList();

            foreach (var pipe in pipes)
            {
                string innerDiam = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble(), UnitTypeId.Millimeters).ToString();
                string outerDiam = UnitUtils.ConvertFromInternalUnits(pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble(), UnitTypeId.Millimeters).ToString();

                using (Transaction ts = new Transaction(doc, "Set parameter"))
                {
                    ts.Start();

                    Parameter NameParameter = pipe.LookupParameter("Наименование");
                    NameParameter.Set($"Труба {innerDiam}/{outerDiam}");

                    ts.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
