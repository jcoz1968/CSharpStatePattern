using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using State_Design_Pattern.UI;

namespace State_Design_Pattern.Logic
{
    public class Booking
    {
        private MainWindow View { get;  set; }
        public string Attendee { get; set; }
        public int TicketCount { get; set; }
        public int BookingID { get; set; }

        private CancellationTokenSource cancelToken;

        private bool isNew { get; set; }
        private bool isPending { get; set; }
        private bool isBooked { get; set; }

        public Booking(MainWindow view)
        {
            isNew = true;
            View = view;
            BookingID = new Random().Next();
            ShowState("New");
            View.ShowEntryPage();
        }

        public void SubmitDetails(string attendee, int ticketCount)
        {
            if(isNew)
            {
                isNew = false;
                isPending = true;
                Attendee = attendee;
                TicketCount = ticketCount;

                cancelToken = new CancellationTokenSource();
                StaticFunctions.ProcessBooking(this, ProcessingComplete, cancelToken);

                ShowState("Pending");
                View.ShowStatusPage("Processing Booking");
            }

        }

        public void Cancel()
        {
            if(isNew)
            {
                ShowState("Closed");
                View.ShowStatusPage("CanceledByUser");
                isNew = false;
            }
            else if(isPending)
            {
                cancelToken.Cancel();
            }
            else if (isBooked)
            {
                ShowState("Closed");
                View.ShowStatusPage("Booking canceled: Expect a refund");
                isBooked = false;
            }
            else
            {
                View.ShowError("Closed bookings cannot be canceled");
            }
        }

        public void DatePassed()
        {
            if(isNew)
            {
                ShowState("Closed");
                View.ShowStatusPage("Booking expired");
                isNew = false;
            }
            else if(isBooked)
            {
                ShowState("Closed");
                View.ShowStatusPage("We hope you enjoy the event");
                isBooked = false;
            }
        }

        public void ProcessingComplete(Booking booking, ProcessingResult result)
        {
            isPending = false;
            switch (result)
            {
                case ProcessingResult.Sucess:
                    ShowState("Booked");
                    View.ShowStatusPage("Enjoy the Event");
                    isBooked = true;
                    break;
                case ProcessingResult.Fail:
                    View.ShowProcessingError();
                    Attendee = string.Empty;
                    BookingID = new Random().Next();
                    ShowState("New");
                    isNew = true;
                    View.ShowEntryPage();
                    break;
                case ProcessingResult.Cancel:
                    ShowState("Closed");
                    View.ShowStatusPage("Processing Canceled");
                    break;
            }
        }

        public void ShowState(string stateName)
        {
            View.grdDetails.Visibility = System.Windows.Visibility.Visible;
            View.lblCurrentState.Content = stateName;
            View.lblTicketCount.Content = TicketCount;
            View.lblAttendee.Content = Attendee;
            View.lblBookingID.Content = BookingID;
        }

      

    }
}


