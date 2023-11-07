namespace Backend.Helpers.MatrixHelper
{
    public static class MatrixCalculator
    {
        public static DateTime CalculateStartTime(DateTime date, int slot)
        {
            DateTime startTime;

            if (slot == 0)
            {
                startTime = date.Date.AddHours(7);
            }
            else if (slot == 1)
            {
                startTime = date.Date.AddHours(9);
            }
            else if (slot == 2)
            {
                startTime = date.Date.AddHours(13);
            }
            else
            {
                startTime = date.Date.AddHours(15);
            }

            return startTime;
        }
    }
}
