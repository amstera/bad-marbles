extern UIViewController* UnityGetGLViewController();

extern "C" void _Native_Share_iOS(const char* file, const char* message)
{
    NSMutableArray *items = [NSMutableArray new];
    NSString *filePath = [NSString stringWithUTF8String:file];
    NSString *shareMessage = [NSString stringWithUTF8String:message];
    UIImage *image = [UIImage imageWithContentsOfFile:filePath];

    if (image != nil)
        [items addObject:image];
    else
        [items addObject:[NSURL fileURLWithPath:filePath]];

    if (shareMessage != nil)
        [items addObject:shareMessage];

    UIActivityViewController *activity = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
    UIViewController *controller = UnityGetGLViewController();
    controller.modalPresentationStyle = UIModalPresentationPopover;
    controller.popoverPresentationController.sourceView = controller.view;
    controller.popoverPresentationController.sourceRect = CGRectMake(controller.view.frame.size.width / 2, controller.view.frame.size.height / 4, 0, 0);

    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
        [controller presentViewController:activity animated:YES completion:nil];
    else
    {
        UIActivityViewController *contentViewController = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
        contentViewController.preferredContentSize = CGSizeMake(100,100);
        contentViewController.modalPresentationStyle = UIModalPresentationPopover;
        contentViewController.popoverPresentationController.sourceView = activity.view;
        [controller presentViewController:contentViewController animated:YES completion:nil];
    }
}