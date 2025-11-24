
Devin Puckett

CST-250 Programming in C#2

Grand Canyon University

11/11/2025

Milestone 6

[https://github.com/devinpuckett4/CST-250-Milestone-6/blob/main/Milestone6.md]

https://www.loom.com/share/ef05117809ff4b2a9acc392119092e2c












 

FLOW CHART

  
Figure 1: Flow chart of Milestone 6

This flowchart shows the full path of my Minesweeper game from start to finish. The game begins on FormStart, where the player chooses the board size and difficulty, then clicks Start Game to move into FormGame with the grid, timer, and optional peek feature. From there, the player clicks or flags cells, which triggers the RevealCell logic, flood fill for empty spaces, and a constant check to see if the game is won or lost. If the player hits a bomb, the flow goes to the Lose step, where all bombs are shown and the game ends. If the player successfully clears the board, it follows the Win path to display the score, prompt for their name, create a GameStat record, and update the High Scores screen with options to save, load, or sort scores before ending the game.



UML Class Diagram
  
Figure 2: UML Class Diagram

This diagram lays out how my Minesweeper game is organized using separate models, forms, and services. BoardModel and CellModel handle the core game data, including the grid, timer, difficulty, neighbors, and whether a cell is flagged, revealed, or a reward. GameState and GameStat track whether the player is still playing, won, or lost, and store the final results like score, time, and name. FormStart, FormGame, Form3, and Form4 control the user experience: starting the game, playing on the grid, entering the player’s name, and viewing or managing high scores. The IBoardOperations interface and BoardService class sit in the middle to perform actions like setting bombs, revealing cells, counting neighbors, toggling flags, and calculating the final score, keeping the game logic separate from the UI and easier to manage.


Low Fidelity

  
 
Figure 3: Screenshot of Build Success

This screenshot shows my full Minesweeper solution building successfully from the command line. After navigating into the Minesweeper folder, I run dotnet build and all projects compile with no errors. The Models and BLL projects succeed first, confirming that my core data structures and game logic are solid. The ConsoleApp, Tests, and WinForms projects all build cleanly as well, which means both interfaces and my automated tests are wired up correctly. Seeing “Build succeeded” at the bottom for every layer confirms the application is stable and ready to run or demonstrate.











High Fidelity
 

Figure 4: High Fidelity

 
This screenshot shows the results of running my automated tests for the Minesweeper solution using dotnet test. The Models, BLL, and Tests projects all build successfully first, confirming that the structure and references are set up correctly. xUnit then discovers and runs the Minesweeper.Tests project without any issues. The test summary reports four tests total, all succeeded with zero failures or skips, which gives me confidence that my core game logic is behaving as expected. Seeing “Build succeeded” at the end confirms that both compilation and testing passed cleanly, so the project is ready to demo.







 
 


Figure 5: Screenshot of Game Setup

 
This screenshot shows the setup screen for my Minesweeper game. At the top I can use a slider to choose the board size, and it is currently set to a ten by ten grid. Below that there is another slider that controls the difficulty level, which is set to two in this example. At the bottom there are two large buttons, one to start the game and one to view the high scores. This screen lets the player pick how big and how hard they want the game to be before they begin.








Figure 6: Screenshot of 24x24 Board
 

This screenshot shows my Minesweeper game running on a large 24x24 board for a more advanced challenge. Every tile starts as a gray button with a question mark so it’s clear that everything is hidden and unexplored. The message at the top reminds the player of the simple controls: left-click to reveal and right-click to flag potential bombs. At the bottom, the Peeks display shows that one safe hint is available, along with Use and Close buttons so the player can decide when to spend it. The layout keeps everything clean and consistent, even at this bigger size, so the focus stays on strategy instead of clutter.





Figure 7: Screenshot of Visit, Fill and Flag

 

This screenshot shows my Minesweeper game running on a 24x24 board in the middle of an active round. A large portion of the board has been revealed, showing numbers and clear spaces that outline safe paths, while the remaining gray question mark tiles still hide possible bombs. A few yellow flags mark suspected bomb locations, helping me track my logic as I work through the grid. The instructions at the top keep controls clear, and at the bottom the Peeks display shows I still have one safe hint available with a Use button ready if I decide to rely on it. Even at this larger size, the layout stays readable and supports strategic play instead of guesswork.





 Figure 8: Screenshot of Loss

 
This screenshot shows the lose screen on my larger 24x24 Minesweeper board. The “Boom! You lost” message at the top, along with the final time of 411 seconds, clearly lets me know the round is over and how long I lasted. All bombs are revealed in red so I can see the full layout and where my mistakes were, while my incorrect guesses are easy to spot. The gray path of opened cells shows how far I had logically worked through the board before hitting a mine. At the bottom, the Peeks display and disabled Use button confirm that no more actions can be taken, and the Close button allows me to exit and start a new attempt. 







Figure 9: Screenshot of Special Reward

 
 
This first screenshot shows a message box that pops up when the player discovers the special reward in my Minesweeper game. The title says Reward Found and the message explains that the player has earned a hint reward and can now use the Use Peek feature safely. This tells the player they unlocked a helpful power that makes the game a little easier for a moment. The OK button closes the message so they can go back to the board and keep playing. It highlights that the game has a fun bonus hidden on the board, not just bombs.
The second screenshot shows the game right after I hit the special reward and earned an extra peek. Before the reward I had five peeks, and now the label shows Peeks 6 to confirm it went up by one. The board is still in progress with some tiles open and some covered, but the main point here is that the reward worked and added another safe peek I can use. This proves that the reward logic is tied into the peek counter and updates the player’s total during the game.





Figure 10: Screenshot of High Scores Page

 
This screenshot shows the High Scores window from my Minesweeper game. The table lists each player’s name, their score, how long their game took, and the date the score was recorded. You can see several sample scores already loaded, including my own name in the list. At the bottom of the window the program also shows the average score and average time across all the results, which gives a quick summary of how players are doing overall. This screen proves that the game is tracking results and displaying them in a clear way after the games are finished.










Figure 11: Screenshot of High Scores Page

 
This screenshot shows the High Scores window from my Minesweeper game. The table lists each player’s name, their score, how long their game took, and the date the score was recorded. You can see several sample scores already loaded, including my own name in the list. At the bottom of the window the program also shows the average score and average time across all the results, which gives a quick summary of how players are doing overall. This screen proves that the game is tracking results and displaying them in a clear way after the games are finished.







Figure 12: Screenshot of High Scores Page Sorted
 
The High Scores page is currently sorted by the Name column. You can tell because the list of players is in alphabetical order by first name: Brandon, Devin, Jimmy, Maynard, Samantha, Scott, Seth, and Steve. The scores and times are not in order; they move with the player’s name when the list is sorted. When the user chooses the “Sort” option, the program takes the list of high-score records and orders them by the player’s name before showing them in the grid. So the sorting only changes how the records are displayed, not the data itself. At the bottom of the form, the program also shows the Average Score and Average Time for all the records; those values stay the same no matter how the list is sorted.









---
Use Case Scenario
---

 
This diagram shows the end-to-end user flow for my Minesweeper app, including the win path into high scores. From the User lane, I start the game, view the board, and make moves; those actions feed into Determine_Gamestate, which decides the result. The results screen shows win or loss, and on a win I’m prompted to enter my name. That name then flows into the View High Scores screen so the round is recorded and visible. The System lane covers engine actions like visit/flag/peek, and the Administrator lane handles setup and quality, setting board size, difficulty percentage, and running xUnit tests to keep everything reliable.

ADD ON
Programming Conventions
1.	Keep files grouped by what they do. Models hold the data, the BLL handles the game rules and rewards, and the WinForms app shows the board, averages, and reads input.
2.	Name things clearly and add small comments so future me knows what a method is for, especially around the average math and reward odds.
3.	The BLL is the brain. The models are storage. The UI only displays and reads from the player (for example, Form4 just asks the BLL for averages instead of doing the math itself).
4.	Keep methods focused on one job. RevealCell still handles regular visits and calls flood fill when needed. New methods include CalculateAverageScore, CalculateAverageTime, TryTriggerRewardPeek, and ApplyRewardPeekToBoard.
5.	Check for out-of-bounds or bad input first, then always guide the player with a friendly message (for example, no stats yet, or reward already used this game).
Computer Specs
• Windows 10 or Windows 11
• Visual Studio 2022
• .NET SDK installed
• 8 GB RAM or more
• Git and GitHub account
Work Log Milestone 6
Wednesday • 4:05–5:25 PM – Added BLL methods for CalculateAverageScore and CalculateAverageTime and bound them to labels on the high scores form. Total: 1h 20m
Friday • 6:10–7:30 PM – Implemented the special reward peek logic and wired it into the existing reward system. Total: 1h 20m
Saturday • 9:45–11:10 AM – Updated the board drawing code so the reward peek is actually visible on the board (temporary reveal) and cleans itself up afterward. Total: 1h 25m
Saturday • 1:15–2:20 PM – Tuned the reward odds so the peek shows up more often but not every game; moved the odds into a constant for easy tweaking. Total: 1h 05m
Sunday • 5:30–6:50 PM – Tested averages with different sets of stats, checked reward behavior across multiple games, and retook screenshots. Total: 1h 20m
Grand Total: 6h 30m
What I added
• Average score and average time display at the bottom of the high scores form, updating whenever stats are loaded.
• BLL methods to calculate averages from the GameStat list instead of the UI doing the math.
• Special reward peek fully implemented so, when triggered, it reveals extra information on the board for the player.
• Increased reward odds so the peek feels special but shows up a bit more often, and made the odds configurable.
Research
1.	YouTube – examples of TimeSpan formatting and calculating averages in C#.
2.	Articles on game design that talked about balancing reward frequency so bonuses feel exciting but not overpowered.
3.	Posts about Minesweeper-style “hint” or “peek” features and how they can help newer players.
4.	Quick searches on LINQ to confirm the cleanest way to average scores and times from a list of objects.
OOP principles I used
• Abstraction – The board/reward interface only describes what actions are available (like TryTriggerRewardPeek and UseRewardPeek), not how they are implemented.
• Encapsulation – Average math and reward odds stay hidden inside the BLL; forms just call methods and display the results.
• Polymorphism – The reward system uses the same pattern for different reward types, and the sorting still switches between name/score/time.
• Inheritance – New and existing forms inherit from Form so they share consistent window behavior and styling.
Tests
• Verified average score and average time calculate correctly with multiple GameStat entries.
• Tested that averages handle the “no stats yet” case without crashing (shows 0 or placeholder text).
• Confirmed the reward peek triggers at the new odds and only once per game.
• Checked that the peek reveals the correct cells on the board and then returns the board to normal.
Bug Report
• Averages dividing by zero when there were no saved stats; fixed by checking the count first and returning 0 when the list is empty.
• Reward peek state not resetting between games; added a reset in the NewGame setup so each game starts clean.
• Peek highlight sometimes staying on the board after the move; updated the redraw logic to clear the temporary reveal once the peek is used.
Follow-Up Questions
1.	What was tough?
Getting the reward peek to show on the board in a clear way without breaking normal reveal logic was tricky, and I had to be careful about the timing.
2.	What did I learn?
I learned how to calculate and display averages from stored stats and how small changes to reward odds can really change how the game “feels” to the player.
3.	How would I improve it?
I would add a simple settings screen so players can toggle the reward on/off, and maybe show how many times they have used the peek.
4.	How does this help at work?
Handling averages and rewards cleanly is similar to tracking user metrics and bonus features in real apps. It reinforces separating UI from logic and makes it easier to adjust features without rewriting everything.

