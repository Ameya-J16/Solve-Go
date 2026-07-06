# Quiz Questions

This folder holds the multiple-choice quiz questions used by the quiz stages, stored as Unity **ScriptableObject** assets (`Question1` … `Question 12`).

Each question asset contains:

- `question` — the question text shown to the player
- `answers` — a list of four answer choices
- `CorrectAnswerIndex` — zero-based index of the correct answer in the list

Example (`Question1.asset`):

> **'OS' computer abbreviation usually means?**
> 1. Order of Significance
> 2. Open Software
> 3. Optical Sensor
> 4. Operating System ✅ (`CorrectAnswerIndex: 3`)

## Adding a New Question

1. In the Unity editor, right-click in this folder and create a new Question asset (via the Question ScriptableObject's **Create** menu entry).
2. Fill in the question text and the four answers in the Inspector.
3. Set `CorrectAnswerIndex` to the position of the right answer (counting from 0).
4. Add the asset to the quiz manager's question list in the quiz scene.
