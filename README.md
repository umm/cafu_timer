# cafu_timer

## What

- TimerUseCase based on CAFU architecture

## Requirement

- cafu\_core
- event\_activator
- unity\_module\_stopwatch

## Install

```shell
yarn add "umm/cafu_timer#^1.0.0"
```

## Usage

Implement ITimerPresenter on your presenter.

```csharp

public class MyPresenter : ITimerPresenter
{
    public ITimerUseCase TimerUseCase { get; private set; }
}
```

Attach TimerGauge or TimerText on your image or text.
That's all you have to do to use timer.

## License

Copyright (c) 2018 Takuma Maruyama

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

