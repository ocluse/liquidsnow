﻿using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class ContainerBase : ControlBase
    {
        [Parameter]
        public int State { get; set; } = ContainerState.Found;

        [Parameter]
        public EventCallback<int> StateChanged { get; set; }

        [Parameter]
        public RenderFragment? EmptyTemplate { get; set; }

        [Parameter]
        public RenderFragment? LoadingTemplate { get; set; }

        [Parameter]
        public RenderFragment<Exception?>? ErrorTemplate { get; set; }

        [Parameter]
        public RenderFragment? UnauthorizedTemplate { get; set; }

        [Parameter]
        public RenderFragment? NotFoundTemplate { get; set; }

        [Parameter]
        public RenderFragment? ReauthenticationRequiredTemplate { get; set; }

        private Exception? _exception;

        protected void BuildContainer(RenderTreeBuilder builder)
        {
            if (State == ContainerState.Found)
            {
                BuildFound(builder);
            }
            else if (EmptyTemplate != null && State == ContainerState.Empty)
            {
                builder.AddContent(360, EmptyTemplate);
            }
            else if (LoadingTemplate != null && State == ContainerState.Loading)
            {
                builder.AddContent(361, LoadingTemplate);
            }
            else if (State == ContainerState.Error && State == ContainerState.Error)
            {
                builder.AddContent(362, ErrorTemplate, _exception);
            }
            else if (NotFoundTemplate != null && State == ContainerState.NotFound)
            {
                builder.AddContent(363, NotFoundTemplate);
            }
            else if (UnauthorizedTemplate != null && State == ContainerState.Unauthorized)
            {
                builder.AddContent(364, UnauthorizedTemplate);
            }
            else if (ReauthenticationRequiredTemplate != null && State == ContainerState.ReauthenticationRequired)
            {
                builder.AddContent(365, ReauthenticationRequiredTemplate);
            }
            else
            {
                Type typeToRender = Resolver.ResolveContainerStateToRenderType(State);
                builder.OpenComponent(166, typeToRender);
                builder.CloseComponent();
            }
        }

        protected abstract void BuildFound(RenderTreeBuilder builder);

        protected abstract Task<int> FetchDataAsync();

        public async Task ReloadDataAsync()
        {
            await UpdateStateAsync(ContainerState.Loading);
            _exception = null;
            int newState;
            
            try
            {
                newState = await FetchDataAsync();
                
            }
            catch (Exception ex)
            {
                _exception = ex;

                if (ErrorTemplate != null)
                {
                    newState = ContainerState.Error;
                }
                else
                {
                    newState = VenusResolver.ResolveExceptionToContainerState(ex);
                }
            }

            await UpdateStateAsync(newState);
        }

        protected async Task UpdateStateAsync(int newState)
        {
            if (newState != State)
            {
                State = newState;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
