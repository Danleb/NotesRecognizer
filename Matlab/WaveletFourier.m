tStart = -10;
tEnd = 10;
t = [tStart:0.001:tEnd];

f = 0.5;
s = 1;
sine = exp(1i*2*pi*f*t);
gaus = exp(-(t/s).^2/2);
cmw = sine .* gaus;
plot(t, cmw);
%spectrogram(cmw);
spectrogram(cmw,kaiser(64,18),50,128,1e3,'yaxis');
ylim([0 110])

%y = sin(100*t);
%y = (1-t.^2).*exp((-t.^2)/2);

%plot(t, y);
%spectrogram(y,'yaxis');
%spectrogram(y,kaiser(64,18),50,128,1e3,'yaxis');

