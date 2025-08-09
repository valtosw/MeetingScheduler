
# Meeting Scheduler

## Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://github.com/valtosw/meeting-scheduler.git
   cd meeting-scheduler
   ```

2. **Build & run the Web API**

   ```bash
   dotnet restore
   dotnet build
   dotnet run --project MeetingScheduler.WebAPI
   ```

3. **Use Swagger for testing**

   ```
   https://localhost:<port>/swagger
   ```

   To interact with the three endpoints:

   * `POST /users` – creates a user
   * `POST /meetings` – find and schedule a meeting (returns the earliest time slot that fits all users' calendars)
   * `GET /users/{userId}/meetings` – returns all meetings for a user

## Known Limitations & Edge Cases

* **No weekend exclusion**: Meetings can be scheduled on weekends.
* **No input validation**: There is no input validation like `durationMinutes > 0` etc.
