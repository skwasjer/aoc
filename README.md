# Advent Of Code solutions

This repo contains my personal solutions to [Advent Of Code](https://adventofcode.com/) puzzles.

## Puzzles

- Each year is solved in its own project.
- Each project has a class for each day, which implements `IPuzzle` (or the `Puzzle<>` base class).
- Each daily puzzle class has implementations for the first and second part, annotated with a `SolutionAttribute`. This attribute is a custom NUnit attribute allowing the dotnet test runner to be used to assert the solution/implementation is correct. The attribute takes care of:
  - Resolving the test method(s) for the test runner.
  - Loading the input data (from `IPuzzle.GetInput()`) and passing it as test parameter(s). By default, the data is loaded from text files in the `input/` folder.
  - Asserting the method return parameter against a known puzzle outcome/result. This is provided as attribute parameter but can also be loaded from user secrets (see below).

## Testing

Run `dotnet test` to validate all solutions.

This git repository does not include the actual puzzle input and expected solutions. It only includes the example input and its expected solution. The example input is however not exhaustive and can result in false positives.

To avoid false positives, the puzzle implementations should be validated using real input data from the Advent of Code website. To use the actual input and assert the real expected solutions, we can override the example data:

- Under each project, in the `inputs/` folder, add an `e2e` folder holding the real input files, eg. `inputs/e2e/day01.txt`, `inputs/e2e/day02.txt`, etc.
- Next, add a user secrets JSON file with the expected solutions.

The expected format for user secrets is:

```jsonc
{
  "day01":
  {
    "part1": 123456,
    "part2": 123456
  },
  "day02":
  {
    "part1": 123456,
    "part2": 123456
  },
  // etc
}
```
