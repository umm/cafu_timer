# cafu_timer

## What

- TimerUseCase based on CAFU architecture

## Requirement

- cafu\_core

## Install

```shell
yarn add "umm-projects/cafu_timer#^1.0.0"
```

## Usage

Implement your presenter.

```csharp

public class MyPresenter : ITimerPresenter
{
    public ITimerUseCase TimerUseCase { get; private set; }
}
```

Attach TimerGauge on your image which fills image
Now you created your timer gauge!

## License

Copyright (c) 2018 Takuma Maruyama

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

