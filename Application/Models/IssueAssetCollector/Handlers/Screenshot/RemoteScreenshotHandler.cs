﻿using BocchiTracker.ApplicationInfoCollector;
using BocchiTracker.ModelEvent;
using BocchiTracker.ProcessLinkQuery.Queries;
using Prism.Events;
using SixLabors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Handlers.Screenshot
{
    public class RemoteScreenshotSaveProcess
    {
        public string Output { get; set; } = string.Empty;

        private IEventAggregator _mediator;

        public RemoteScreenshotSaveProcess(IEventAggregator inMediator) 
        {
            _mediator = inMediator;
            _mediator
                .GetEvent<ReceiveScreenshotEvent>()
                .Subscribe(Handle, ThreadOption.BackgroundThread);
        }

        public void Handle(ReceiveScreenshotEventParameter inRrequest)
        {
            var data = inRrequest;
            using var image = Image.LoadPixelData<Byte4>(data.ImageData, data.Width, data.Height);
            
            image.SaveAsPng(Output);
        }
    }

    public class RemoteScreenshotHandler : ScreenshotHandler
    {
        private IEventAggregator _eventAggregator;
        public RemoteScreenshotSaveProcess SaveProcess { get; private set; }

        public RemoteScreenshotHandler(IEventAggregator inEventAggregator, IFilenameGenerator inFilenameGenerator)
            : base(inFilenameGenerator)
        {
            this._eventAggregator = inEventAggregator;
            this.SaveProcess = new RemoteScreenshotSaveProcess(inEventAggregator);
        }

        public override void Handle(int inClientID, int inPID, string inOutput)
        {
            this.SaveProcess.Output = Path.Combine(inOutput, _filenameGenerator.Generate() + ".png");

            _eventAggregator
                .GetEvent<RequestQueryEvent>()
                .Publish(new RequestQueryEventParameter(inClientID, QueryID.ScreenshotData));
        }
    }
}
